using HashidsNet;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;
public class SearchController : Controller
{
	private readonly IHashids _hashids;
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

    public SearchController(IHashids hashids, IMapper mapper, ApplicationDbContext context)
    {
        _hashids = hashids;
        _mapper = mapper;
        _context = context;
    }

    public IActionResult Index()
	{
		return View();
	}

	public IActionResult Details(string bookKey)
	{
		var bookId = _hashids.Decode(bookKey)[0];

		var book=_context.Books.Include(b=>b.BookCopies).SingleOrDefault(b=>b.Id==bookId);

		return View(_mapper.Map<BookViewModel>(book));
	}
}
