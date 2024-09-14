using Bookify.Application.Common.Services.Books;
using Bookify.Application.Common.Services.RentalCopies;
using Bookify.Application.Common.Services.Subscriper;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers;
[Authorize]
public class DashboardController : Controller
{
    private readonly IBookService _bookService;
    private readonly ISubscriperService _subscriperService;
    private readonly IRentalCopiesService _rentalCopiesService;
    private readonly IMapper _mapper;

    public DashboardController(IMapper mapper, IBookService bookService, ISubscriperService subscriperService, IRentalCopiesService rentalCopiesService)
    {
        _mapper = mapper;
        _bookService = bookService;
        _subscriperService = subscriperService;
        _rentalCopiesService = rentalCopiesService;
    }

    public IActionResult Index()
    {
        var numberOfBooks = _bookService.GetActiveBooksCount();
        var numberOfSubscripers = _subscriperService.GetActiveSubscripersCount();
        var latestBooks = _bookService.GetLatestBooks(8);
        var topBooks = _bookService.GetTopBooks(6);

        DashboardViewModel viewModel = new()
        {
            NumberOfBooks = numberOfBooks,
            NumberOfSubscriber = numberOfSubscripers,
            LatestBooks = _mapper.Map<IEnumerable<BookViewModel>>(latestBooks),
            TopBooks = _mapper.Map<IEnumerable<BookViewModel>>(topBooks)
        };
        return View(viewModel);
    }

    [AjaxOnly]
    public IActionResult GetNumberOfRentalsPerDay(DateTime? startDate, DateTime? endDate)
    {
        startDate ??= DateTime.Today.AddDays(-29);
        endDate ??= DateTime.Today;

        var rentals=_rentalCopiesService.GetRentalsPerDay(startDate, endDate);

        return Ok(rentals);
    }

    public IActionResult GetSubscribersPerCities()
    {
        return Ok(_subscriperService.GetSubscripersPerCity());
    }
}
