using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;
public class RentalsController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;
	private readonly IDataProtector _dataProtector;


	public RentalsController(ApplicationDbContext context, IMapper mapper, IDataProtectionProvider dataProtector)
	{
		_context = context;
		_mapper = mapper;
		_dataProtector = dataProtector.CreateProtector("MySecureKey");
	}

	public IActionResult Create(string subscriperKey)
	{
		var subscriperId=int.Parse(_dataProtector.Unprotect(subscriperKey));
		var subscriper = _context.Subscripers.Include(s=>s.Subscriptions).Include(s=>s.Rentals).ThenInclude(r=>r.RentalCopies).SingleOrDefault(s=>s.Id==subscriperId);

		if (subscriper is null)
			return NotFound();

		var availableCopies = subscriper.Rentals.SelectMany(r => r.RentalCopies).Count(rc => !rc.ReturnDate.HasValue );
		var maxAllowedCopies=(int)RentalsConfigurations.MaxAllowedCopies - availableCopies;

		if (maxAllowedCopies.Equals(0))
			return View("NotAllowedRental", Errors.MaxAllowedCopies);
		if (subscriper.IsBlackListed)
			return View("NotAllowedRental", Errors.BlackListedSubscriber);

		if (subscriper.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalsConfigurations.MaxAllowedCopies))
			return View("NotAllowedRental", Errors.InActiveSubscriber);

		var viewModel = new RentalFormViewModel
		{
			MaxAllowedCopies = maxAllowedCopies,
		};

        return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
    public IActionResult Create(RentalFormViewModel viewModel)
    {

        return View(viewModel);
    }

    [HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult GetBookCopy(SearchFormViewModel viewModel)
	{
		if (!ModelState.IsValid)
			return BadRequest();

		var copy = _context.BookCopies.Include(c => c.Book).SingleOrDefault(c => c.SerialNumber.ToString().Equals(viewModel.Value) && c.IsActive && c.Book!.IsActive);

		if (copy is null)
			return BadRequest("Invalid serial number");

		if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
			return BadRequest("This book/copy is not available for rental");

		// Check that copy is not in rental with another person
		var copyIsInRental = _context.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);

		if(copyIsInRental)
			return BadRequest(Errors.CopyInRental);


		var model =_mapper.Map<BookCopyViewModel>(copy);	
		return PartialView("_RentalCopyDetails",model);
	}
}
