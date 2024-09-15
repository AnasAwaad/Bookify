using Bookify.Application.Common.Services.Books;
using HashidsNet;
using Microsoft.AspNetCore.WebUtilities;

namespace Bookify.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly IHashids _hashids;


        public HomeController( IMapper mapper, IHashids hashids, IBookService bookService)
        {
            _mapper = mapper;
            _hashids = hashids;
            _bookService = bookService;
        }

        public IActionResult Index()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction(nameof(Index), "Dashboard");

            
            var latestBooks = _bookService.GetLatestBooks(10);

            var viewModel = _mapper.Map<IEnumerable<BookViewModel>>(latestBooks);

            foreach (var book in viewModel)
                book.Key = _hashids.EncodeHex(book.Id.ToString());

            return View(viewModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statusCode = 500)
        {
            return View(new ErrorViewModel { ErrorDescription = ReasonPhrases.GetReasonPhrase(statusCode) });
        }
    }
}
