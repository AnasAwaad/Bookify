using Bookify.Application.Common.Services.Books;
using HashidsNet;

namespace Bookify.Web.Controllers;
public class SearchController : Controller
{
    private readonly IHashids _hashids;
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public SearchController(IHashids hashids, IMapper mapper, IBookService bookService)
    {
        _hashids = hashids;
        _mapper = mapper;
        _bookService = bookService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult FindBook(string query)
    {
        
        var books=_bookService.Search(query);
        var viewModel = _mapper.ProjectTo<BookSearchResaultViewModel>(books).ToList();

        foreach (var book in viewModel)
        {
            book.Key = _hashids.EncodeHex(book.Id.ToString());
        }

        return Ok(viewModel);
    }

    public IActionResult Details(string bookKey)
    {
        var bookId = _hashids.DecodeHex(bookKey);

        var book = _bookService.GetDetails();

        var viewModel=_mapper.ProjectTo<BookViewModel>(book)
            .SingleOrDefault(b => b.Id == int.Parse(bookId) && b.IsActive);
        
        if (viewModel is null)
            return NotFound();

        return View(viewModel);
    }
}
