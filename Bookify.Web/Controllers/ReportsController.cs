using Bookify.Web.Core.Utilities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers;

[Authorize(Roles =AppRoles.Admin)]
public class ReportsController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

    public ReportsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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
			viewModel.Data = PaginatedList<Book>.Create(books, pageNumber.Value, 20);

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

		for (int i = 0; i < headerCells.Length; i++)
		{
			sheet.Cell(1, i + 1).SetValue(headerCells[i]);

		}

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

		var header = sheet.Range(1,1,1,headerCells.Length);

		header.Style.Fill.BackgroundColor = XLColor.Black;
		header.Style.Font.FontColor= XLColor.White;
		header.Style.Font.SetBold();
		header.Style.Font.FontSize = 14;


		sheet.ColumnsUsed().AdjustToContents();
		sheet.Style.Alignment.Horizontal=XLAlignmentHorizontalValues.Center;


		sheet.CellsUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
		sheet.CellsUsed().Style.Border.OutsideBorderColor = XLColor.Black;

		await using var stream = new MemoryStream();
		wb.SaveAs(stream);

		return File(stream.ToArray(),"application/octet-stream","Books.xlsx");
	}
}
