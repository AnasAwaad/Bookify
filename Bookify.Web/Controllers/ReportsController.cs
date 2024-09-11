using Bookify.Web.Core.Models;
using Bookify.Web.Core.Utilities;
using Bookify.Web.Extenstions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpenHtmlToPdf;
using ViewToHTML.Services;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bookify.Web.Controllers;

[Authorize(Roles = AppRoles.Admin)]
public class ReportsController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;
	private readonly IViewRendererService _viewRendererService;

	public ReportsController(ApplicationDbContext context, IMapper mapper, IViewRendererService viewRendererService)
	{
		_context = context;
		_mapper = mapper;
		_viewRendererService = viewRendererService;
	}

	public IActionResult Index()
	{
		return View();
	}

	public IActionResult Book(int? pageNumber, List<int> selectedCategories, List<int> selectedAuthors)
	{
		var authors = _context.Authors.OrderBy(a => a.Name).ToList();
		var categories = _context.Categories.OrderBy(c => c.Name).ToList();
		IQueryable<Book> books = _context.Books
			.Include(b => b.Author)
			.Include(b => b.Categories)
			.ThenInclude(c => c.Category)
			.Where(b => (!selectedCategories.Any() || b.Categories.Any(c => selectedCategories.Contains(c.CategoryId)))
				  && (!selectedAuthors.Any() || selectedAuthors.Contains(b.AuthorId)));

		var viewModel = new BooksReportViewModel()
		{
			Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
			Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
			SelectedAuthors = selectedAuthors,
			SelectedCategories = selectedCategories
		};

		if (pageNumber is not null)
			viewModel.Data = PaginatedList<Book>.Create(books, pageNumber.Value, (int)ReportsConfiguration.pageSize);

		return View(viewModel);
	}


	public async Task<IActionResult> ExportBooksToExcel(string authors, string categories)
	{

		var selectedAuthors = authors?.Split(',');
		var selectedCategories = categories?.Split(',');

		var books = _context.Books
			.Include(b => b.Author)
			.Include(b => b.Categories)
			.ThenInclude(c => c.Category)
			.Where(b => (selectedCategories == null || b.Categories.Any(c => selectedCategories.Contains(c.CategoryId.ToString())))
				  && (selectedAuthors == null || selectedAuthors.Contains(b.AuthorId.ToString())))
			.ToList();

		using var wb = new XLWorkbook();

		var sheet = wb.AddWorksheet("Books");


		var headerCells = new string[] { "Title", "Author", "Categories", "Publisher", "Publishing Date", "Hall", "Available for rental?", "Status" };
		// extension method
		sheet.AddHeader(headerCells);


		for (int i = 0; i < books.Count; i++)
		{
			sheet.Cell(i + 2, 1).SetValue(books[i].Title);
			sheet.Cell(i + 2, 2).SetValue(books[i].Author!.Name);
			sheet.Cell(i + 2, 3).SetValue(string.Join(", ", books[i].Categories.Select(c => c.Category!.Name)));
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

		var selectedAuthors = authors?.Split(',');
		var selectedCategories = categories?.Split(',');

		var books = _context.Books
			.Include(b => b.Author)
			.Include(b => b.Categories)
			.ThenInclude(c => c.Category)
			.Where(b => (selectedCategories == null || b.Categories.Any(c => selectedCategories.Contains(c.CategoryId.ToString())))
				  && (selectedAuthors == null || selectedAuthors.Contains(b.AuthorId.ToString())))
			.ToList();

		var templatePath = "~/Views/Reports/BooksTemplate.cshtml";

		var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, books);
		var pdf = Pdf.From(html).Content();

		return File(pdf.ToArray(), "application/octet-stream", "Books.pdf");
	}




	public IActionResult Rentals(int pageNumber, string duration)
	{
		var viewModel = new RentalsReportViewModel{ Duration = duration };

		if (!string.IsNullOrEmpty(duration))
		{
			var date = duration.Split(" - ");


			if (!DateTime.TryParse(date[0],out DateTime startDate))
			{
				ModelState.AddModelError("Duration", "Invalid start date");
				return View(viewModel);
			}

			if (!DateTime.TryParse(date[1], out DateTime endDate))
			{
				ModelState.AddModelError("Duration", "Invalid end date");
				return View(viewModel);
			}

			IQueryable<RentalCopy> rentals = _context.RentalCopies
				.Include(r => r.Rental)
				.ThenInclude(r => r!.Subscriper)
				.Include(r => r.BookCopy)
				.ThenInclude(c => c!.Book)
				.ThenInclude(b => b!.Author)
				.OrderBy(r => r.RentalDate)
				.Where(r => r.RentalDate >= startDate && r.RentalDate <= endDate );

			viewModel.Data = PaginatedList<RentalCopy>.Create(rentals, pageNumber, (int)ReportsConfiguration.pageSize);
		}
		ModelState.Clear();
		return View(viewModel);
	}

	public async Task<IActionResult> ExportRentalsToExcel(string? duration)
	{
		if (duration is null)
			return NotFound();

		var date = duration.Split(" - ");


		var rentals = _context.RentalCopies
			.Include(r => r.Rental)
			.ThenInclude(r => r!.Subscriper)
			.Include(r => r.BookCopy)
			.ThenInclude(c => c!.Book)
			.ThenInclude(b => b!.Author)
			.OrderBy(r => r.RentalDate)
			.Where(r => r.RentalDate >= Convert.ToDateTime(date[0]) && r.RentalDate <= Convert.ToDateTime(date[1]))
			.ToList();


		using var wb = new XLWorkbook();

		var sheet = wb.AddWorksheet("Rentals");


		var headerCells = new string[] { "Subscriber ID", "Subscriber Name", "Subscriber Phone", "Book Title", "Book Author", "Rental Date", "End Date", "Return Date", "Extended On" };
		// extension method
		sheet.AddHeader(headerCells);


		for (int i = 0; i < rentals.Count; i++)//@(rental.ReturnDate == null ? " - " : rental.ReturnDate!.Value.ToString("d MMM, yyyy"))
		{
			sheet.Cell(i + 2, 1).SetValue(rentals[i].Rental!.Subscriper!.Id);
			sheet.Cell(i + 2, 2).SetValue(rentals[i].Rental!.Subscriper!.FirstName);
			sheet.Cell(i + 2, 3).SetValue(rentals[i].Rental!.Subscriper!.MobileNumber);
			sheet.Cell(i + 2, 4).SetValue(rentals[i].BookCopy!.Book!.Title);
			sheet.Cell(i + 2, 5).SetValue(rentals[i].BookCopy!.Book!.Author!.Name);
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


		var rentals = _context.RentalCopies
			.Include(r => r.Rental)
			.ThenInclude(r => r!.Subscriper)
			.Include(r => r.BookCopy)
			.ThenInclude(c => c!.Book)
			.ThenInclude(b => b!.Author)
			.OrderBy(r => r.RentalDate)
			.Where(r => r.RentalDate >= Convert.ToDateTime(date[0]) && r.RentalDate <= Convert.ToDateTime(date[1]))
			.ToList();


		var templatePath = "~/Views/Reports/RentalsTemplate.cshtml";

		var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, rentals);
		var pdf = Pdf.From(html).Content();

		return File(pdf.ToArray(), "application/octet-stream", "Rentals.pdf");
	}

}
