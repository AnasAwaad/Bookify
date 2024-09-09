using Bookify.Web.Core.Utilities;
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

	public IActionResult Book(int? pageNumber)
	{
		var authors = _context.Authors.OrderBy(a=>a.Name).ToList();
		var categories=_context.Categories.OrderBy(c=>c.Name).ToList();
        IQueryable<Book> books = _context.Books.Include(b => b.Author).Include(b => b.Categories).ThenInclude(c => c.Category);


        var viewModel = new BooksReportViewModel()
		{
			Authors = _mapper.Map<IEnumerable<SelectListItem>>(authors),
			Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
		};

		if(pageNumber is not null)
			viewModel.Data = PaginatedList<Book>.Create(books, pageNumber.Value, 20);

		return View(viewModel);
	}
}
