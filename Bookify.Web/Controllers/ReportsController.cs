using Bookify.Application.Common.Services.Authors;
using Bookify.Application.Common.Services.Books;
using Bookify.Application.Common.Services.Categories;
using Bookify.Application.Common.Services.RentalCopies;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpenHtmlToPdf;
using ViewToHTML.Services;

namespace Bookify.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class ReportsController : Controller
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IViewRendererService _viewRendererService;
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;
    private readonly IBookService _bookService;
    private readonly IRentalCopiesService _rentalCopiesService;

    public ReportsController(IMapper mapper, IViewRendererService viewRendererService, IAuthorService authorService, ICategoryService categoryService, IBookService bookService, IApplicationDbContext context, IRentalCopiesService rentalCopiesService)
    {
        _mapper = mapper;
        _viewRendererService = viewRendererService;
        _authorService = authorService;
        _categoryService = categoryService;
        _bookService = bookService;
        _context = context;
        _rentalCopiesService = rentalCopiesService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Book(int? pageNumber, List<int> selectedCategories, List<int> selectedAuthors)
    {
        var authors = _authorService.GetActiveAuthors();
        var categories = _categoryService.GetActiveCategories();

        
        var viewModel = new BooksReportViewModel()
        {
            Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
            Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
            SelectedAuthors = selectedAuthors,
            SelectedCategories = selectedCategories
        };

        if(pageNumber is not null)
            viewModel.Data = _bookService.GetPaginatedList(pageNumber.Value!, selectedCategories, selectedAuthors);

        return View(viewModel);
    }


    public async Task<IActionResult> ExportBooksToExcel(string authors, string categories)
    {

        var bookDataRows = _bookService.GetQurableRowData(authors, categories);

        var books=_mapper.ProjectTo<BookViewModel>(bookDataRows).ToList();

        using var wb = new XLWorkbook();

        var sheet = wb.AddWorksheet("Books");


        var headerCells = new string[] { "Title", "Author", "Categories", "Publisher", "Publishing Date", "Hall", "Available for rental?", "Status" };
        // extension method
        sheet.AddHeader(headerCells);


        for (int i = 0; i < books.Count; i++)
        {
            sheet.Cell(i + 2, 1).SetValue(books[i].Title);
            sheet.Cell(i + 2, 2).SetValue(books[i].AuthorName);
            sheet.Cell(i + 2, 3).SetValue(string.Join(", ", books[i].Categories));
            sheet.Cell(i + 2, 4).SetValue(books[i].Publisher);
            sheet.Cell(i + 2, 5).SetValue(books[i].PublishingDate.ToString("d MMM, yyyy"));
            sheet.Cell(i + 2, 6).SetValue(books[i].Hall);
            sheet.Cell(i + 2, 7).SetValue(books[i].IsAvailableForRental ? "Yes" : "No");
            sheet.Cell(i + 2, 8).SetValue(books[i].IsActive ? "Available" : "Not available");
        }

        // extension method
        sheet.FormatCells();

        await using var stream = new MemoryStream();
        wb.SaveAs(stream);

        return File(stream.ToArray(), "application/octet-stream", "Books.xlsx");
    }

    public async Task<IActionResult> ExportBooksToPDF(string authors, string categories)
    {
        var bookDataRows = _bookService.GetQurableRowData(authors, categories);

        var books = _mapper.ProjectTo<BookViewModel>(bookDataRows).ToList();

        var templatePath = "~/Views/Reports/BooksTemplate.cshtml";

        var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, books);
        var pdf = Pdf.From(html).Content();

        return File(pdf.ToArray(), "application/octet-stream", "Books.pdf");
    }

    public IActionResult Rentals(int pageNumber, string duration)
    {
        var viewModel = new RentalsReportViewModel { Duration = duration };

        if (!string.IsNullOrEmpty(duration))
        {
            var date = duration.Split(" - ");


            if (!DateTime.TryParse(date[0], out DateTime startDate))
            {
                ModelState.AddModelError("Duration", "Invalid start date");
                return View(viewModel);
            }

            if (!DateTime.TryParse(date[1], out DateTime endDate))
            {
                ModelState.AddModelError("Duration", "Invalid end date");
                return View(viewModel);
            }

            viewModel.Data = _rentalCopiesService.GetPaginatedList(pageNumber, startDate, endDate);
        }
        ModelState.Clear();
        return View(viewModel);
    }

    public async Task<IActionResult> ExportRentalsToExcel(string? duration)
    {
        if (duration is null)
            return NotFound();

        var date = duration.Split(" - ");


        var rentalsData=_rentalCopiesService.GetQurableRowData(Convert.ToDateTime(date[0]), Convert.ToDateTime(date[1]));

        var rentals = _mapper.ProjectTo<RentalReportRowViewModel>(rentalsData).ToList();

        using var wb = new XLWorkbook();

        var sheet = wb.AddWorksheet("Rentals");


        var headerCells = new string[] { "Subscriber ID", "Subscriber Name", "Subscriber Phone", "Book Title", "Book Author", "Rental Date", "End Date", "Return Date", "Extended On" };
        // extension method
        sheet.AddHeader(headerCells);


        for (int i = 0; i < rentals.Count; i++)//@(rental.ReturnDate == null ? " - " : rental.ReturnDate!.Value.ToString("d MMM, yyyy"))
        {
            sheet.Cell(i + 2, 1).SetValue(rentals[i].SubscriperId);
            sheet.Cell(i + 2, 2).SetValue(rentals[i].SubscriberName);
            sheet.Cell(i + 2, 3).SetValue(rentals[i].MobileNumber);
            sheet.Cell(i + 2, 4).SetValue(rentals[i].BookTitle);
            sheet.Cell(i + 2, 5).SetValue(rentals[i].AuthorName);
            sheet.Cell(i + 2, 6).SetValue(rentals[i].RentalDate.ToString("d MMM, yyyy"));
            sheet.Cell(i + 2, 7).SetValue(rentals[i].EndDate.ToString("d MMM, yyyy"));
            sheet.Cell(i + 2, 8).SetValue(rentals[i].ReturnDate == null ? " - " : rentals[i].ReturnDate!.Value.ToString("d MMM, yyyy"));
            sheet.Cell(i + 2, 9).SetValue(rentals[i].ExtendedOn == null ? " - " : rentals[i].ExtendedOn!.Value.ToString("d MMM, yyyy"));
        }

        // extension method
        sheet.FormatCells();

        await using var stream = new MemoryStream();
        wb.SaveAs(stream);

        return File(stream.ToArray(), "application/octet-stream", "Rentals.xlsx");
    }

    public async Task<IActionResult> ExportRentalsToPDF(string? duration)
    {

        if (duration is null)
            return NotFound();

        var date = duration.Split(" - ");

        var rentalsData = _rentalCopiesService.GetQurableRowData(Convert.ToDateTime(date[0]), Convert.ToDateTime(date[1]));

        var rentals = _mapper.ProjectTo<RentalReportRowViewModel>(rentalsData).ToList();

        var templatePath = "~/Views/Reports/RentalsTemplate.cshtml";

        var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, rentals);
        var pdf = Pdf.From(html).Content();

        return File(pdf.ToArray(), "application/octet-stream", "Rentals.pdf");
    }

}
