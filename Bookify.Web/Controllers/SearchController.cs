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

	public IActionResult FindBook(string query)
	{
		var books=_context.Books
			.Include(b=>b.Author)
			.Where(b=> b.IsActive && (b.Title.Contains(query) || b.Author!.Name.Contains(query)))
			.Select(b => new 
			{
				b.Title,
				Author=b.Author!.Name,
				Image=b.ImageThumbnailUrl,
				Key=_hashids.Encode(b.Id)
			})
			.ToList();

		return Ok(books);
	}

	public IActionResult Details(string bookKey)
	{
		var bookId = _hashids.Decode(bookKey)[0];

		var book=_context.Books
			.Include(b=>b.BookCopies)
			.Include(b=>b.Author)
			.Include(b=>b.Categories)
				.ThenInclude(c=>c.Category)
			.SingleOrDefault(b=>b.Id==bookId && b.IsActive);

		if(book is null)
			return NotFound();

		return View(_mapper.Map<BookViewModel>(book));
	}
}
