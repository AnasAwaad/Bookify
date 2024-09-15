using Bookify.Application.Common.Services.BookCopies;
using Bookify.Application.Common.Services.RentalCopies;
using Bookify.Application.Common.Services.RentalService;
using Bookify.Application.Common.Services.Subscripers;
using Microsoft.AspNetCore.DataProtection;

namespace Bookify.Web.Controllers;
public class RentalsController : Controller
{
    private readonly ISubscriperService _subscriperService;
    private readonly IBookCopyService _bookCopyService;
    private readonly IRentalService _rentalService;
    private readonly IRentalCopiesService _rentalCopiesService;
    private readonly IMapper _mapper;
    private readonly IDataProtector _dataProtector;


    public RentalsController(IMapper mapper, IDataProtectionProvider dataProtector, ISubscriperService subscriperService, IBookCopyService bookCopyService, IRentalService rentalService, IRentalCopiesService rentalCopiesService)
    {
        _mapper = mapper;
        _dataProtector = dataProtector.CreateProtector("MySecureKey");
        _subscriperService = subscriperService;
        _bookCopyService = bookCopyService;
        _rentalService = rentalService;
        _rentalCopiesService = rentalCopiesService;
    }

    public IActionResult Create(string subscriperKey)
    {
        var subscriperId = int.Parse(_dataProtector.Unprotect(subscriperKey));
        var subscriper = _subscriperService.GetSubscriperWithRentals(subscriperId);

        if (subscriper is null)
            return NotFound();

        var (errorMessage, maxAllowedCopies) = _subscriperService.CanRent(subscriperId);

        if (!string.IsNullOrEmpty(errorMessage))
            return View("NotAllowedRental", errorMessage);

        var viewModel = new RentalFormViewModel
        {
            SubscriperKey = subscriperKey,
            MaxAllowedCopies = maxAllowedCopies!.Value,
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult Create(RentalFormViewModel viewModel)
    {
        var subscriperId = int.Parse(_dataProtector.Unprotect(viewModel.SubscriperKey));
        var subscriper = _subscriperService.GetSubscriperWithRentals(subscriperId);

        if (subscriper is null)
            return NotFound();

        var (subscriberErrorMessage, maxAllowedCopies) = _subscriperService.CanRent(subscriperId);

        if (!string.IsNullOrEmpty(subscriberErrorMessage))
            return View("NotAllowedRental", subscriberErrorMessage);


        var (bookErrorMessage,copies) =_bookCopyService.CanBeRentaled(viewModel.SelectedCopies, subscriperId);

        if(!string.IsNullOrEmpty(bookErrorMessage))
            return View("NotAllowedRental", bookErrorMessage);

        _subscriperService.AddRentals(subscriperId, copies!, User.GetUserId());

        return Ok();
    }

    [HttpPost]
    public IActionResult RemoveRental(int id)
    {
        var numOfRentalCopies=_rentalService.RemoveRental(id,User.GetUserId());

        if (numOfRentalCopies is null)
            return NotFound();

        return Ok(numOfRentalCopies);
    }

    [HttpPost]
    public IActionResult GetBookCopy(SearchFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var copy = _bookCopyService.SearchForBookCopy(viewModel.Value);

        if (copy is null)
            return BadRequest(Errors.InvalidSerialNumber);

        if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
            return BadRequest(Errors.NotAvailableForRental);

        // Check that copy is not in rental with another person
        var copyIsInRental = _rentalCopiesService.CopyIsInRental(copy.Id);

        if (copyIsInRental)
            return BadRequest(Errors.CopyInRental);

        var model = _mapper.Map<BookCopyViewModel>(copy);
        return PartialView("_RentalCopyDetails", model);
    }
}
