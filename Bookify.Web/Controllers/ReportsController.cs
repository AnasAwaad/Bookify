using Bookify.Web.Core.Models;
using Bookify.Web.Core.Utilities;
using Bookify.Web.Extenstions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpenHtmlToPdf;
using ViewToHTML.Services;

namespace Bookify.Web.Controllers;

[Authorize(Roles =AppRoles.Admin)]
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

	public IActionResult Book(int? pageNumber,List<int> selectedCategories,List<int> selectedAuthors)
	{
		var authors = _context.Authors.OrderBy(a=>a.Name).ToList();
		var categories=_context.Categories.OrderBy(c=>c.Name).ToList();
		IQueryable<Book> books = _context.Books
			.Include(b => b.Author)
			.Include(b => b.Categories)
			.ThenInclude(c => c.Category)
			.Where(b => (!selectedCategories.Any() || b.Categories.Any(c => selectedCategories.Contains(c.CategoryId)))
				  && (!selectedAuthors.Any() || selectedAuthors.Contains(b.AuthorId)));

		//if (selectedAuthors.Any())
		//{
		//	books = books.Where(b => selectedAuthors.Contains(b.AuthorId));
		//}

		//if (selectedCategories.Any())
		//{
		//	books = books.Where(b => b.Categories.Any(c=>selectedCategories.Contains(c.CategoryId)));
		//}

		var viewModel = new BooksReportViewModel()
		{
			Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
			Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
			SelectedAuthors=selectedAuthors,
			SelectedCategories=selectedCategories
		};

		if(pageNumber is not null)
			viewModel.Data = PaginatedList<Book>.Create(books, pageNumber.Value, (int)ReportsConfiguration.pageSize);

		return View(viewModel);
	}


	public async Task<IActionResult> ExportBooksToExcel(string authors,string categories)
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
			sheet.Cell(i + 2, 3).SetValue(string.Join(", ", books[i].Categories.Select(c=>c.Category!.Name)));
			sheet.Cell(i + 2, 4).SetValue(books[i].Publisher);
			sheet.Cell(i + 2, 5).SetValue(books[i].PublishingDate.ToString("d MMM, yyyy"));
			sheet.Cell(i + 2, 6).SetValue(books[i].Hall);
			sheet.Cell(i + 2, 7).SetValue(books[i].IsAvailableForRental ? "Yes": "No");
			sheet.Cell(i + 2, 8).SetValue(books[i].IsActive ? "Available":"Not available");
		}

		// extension method
		sheet.FormatCells();

		await using var stream = new MemoryStream();
		wb.SaveAs(stream);

		return File(stream.ToArray(),"application/octet-stream","Books.xlsx");
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

}
