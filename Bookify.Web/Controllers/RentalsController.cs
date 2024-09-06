using Bookify.Web.Core.Models;
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

		var (errorMessage, maxAllowedCopies) = validateSubscriper(subscriper);

		if (!string.IsNullOrEmpty(errorMessage))
			return View("NotAllowedRental", errorMessage);

		var viewModel = new RentalFormViewModel
		{
			SubscriperKey=subscriperKey,
			MaxAllowedCopies = maxAllowedCopies!.Value,
		};

        return View(viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
    public IActionResult Create(RentalFormViewModel viewModel)
    {
		var subscriperId = int.Parse(_dataProtector.Unprotect(viewModel.SubscriperKey));
		var subscriper = _context.Subscripers
			.Include(s => s.Subscriptions)
			.Include(s => s.Rentals)
				.ThenInclude(r => r.RentalCopies)
			.SingleOrDefault(s => s.Id == subscriperId);

		if (subscriper is null)
			return NotFound();

		var (errorMessage, maxAllowedCopies) = validateSubscriper(subscriper);

		if (!string.IsNullOrEmpty(errorMessage))
			return View("NotAllowedRental", errorMessage);

		var selectedCopies = _context.BookCopies
			.Include(c => c.Book)
			.Include(c=>c.RentalCopies)
			.Where(c=>viewModel.SelectedCopies.Contains(c.SerialNumber))
			.ToList();

		var currentSubscriperRentals = _context.Rentals
			.Include(r => r.RentalCopies)
			.ThenInclude(c => c.BookCopy)
			.Where(r => r.SubscriperId == subscriperId)
			.SelectMany(r => r.RentalCopies)
			.Where(c => !c.ReturnDate.HasValue)
			.Select(r => r.BookCopy!.BookId);

		List<RentalCopy> copies = new();
		foreach (var copy in selectedCopies)
		{
			if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
				return View("NotAllowedRental", Errors.NotAvailableForRental);

			if(copy.RentalCopies.Any(r=>!r.ReturnDate.HasValue))
				return View("NotAllowedRental",Errors.CopyInRental);

			if (currentSubscriperRentals.Any(bookId => bookId == copy.BookId))
				return View("NotAllowedRental", $"This subscriber already has a copy for '{copy.Book.Title}' Book");

			copies.Add(new RentalCopy{ BookCopyId = copy.Id });
		}

		Rental rental = new()
		{
			RentalCopies = copies,
			CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
		};

		subscriper.Rentals.Add(rental);
		_context.SaveChanges();

		return Ok();
    }

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult RemoveRental(int id)
	{
		var rental = _context.Rentals.Find(id);

		if (rental is null || rental.CreatedOn.Date != DateTime.Today)
			return NotFound();

		rental.IsActive = false;
		rental.LastUpdatedOn = DateTime.Today;
		rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

		_context.SaveChanges();
		return Ok();
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult GetBookCopy(SearchFormViewModel viewModel)
	{
		if (!ModelState.IsValid)
			return BadRequest();

		var copy = _context.BookCopies.Include(c => c.Book).SingleOrDefault(c => c.SerialNumber.ToString().Equals(viewModel.Value) && c.IsActive && c.Book!.IsActive);

		if (copy is null)
			return BadRequest(Errors.InvalidSerialNumber);

		if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
			return BadRequest(Errors.NotAvailableForRental);

		// Check that copy is not in rental with another person
		var copyIsInRental = _context.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);

		if(copyIsInRental)
			return BadRequest(Errors.CopyInRental);


		var model =_mapper.Map<BookCopyViewModel>(copy);	
		return PartialView("_RentalCopyDetails",model);
	}

	private (string? errorMessage,int? maxAllowedCopies) validateSubscriper(Subscriper subscriper)
	{
		var availableCopies = subscriper.Rentals.SelectMany(r => r.RentalCopies).Count(rc => !rc.ReturnDate.HasValue);
		var maxAllowedCopies = (int)RentalsConfigurations.MaxAllowedCopies - availableCopies;

		if (maxAllowedCopies.Equals(0))
			return (errorMessage: Errors.MaxAllowedCopies,null);

		if (subscriper.IsBlackListed)
			return (errorMessage: Errors.BlackListedSubscriber, null);


		if (subscriper.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalsConfigurations.MaxAllowedCopies))
			return (errorMessage: Errors.InActiveSubscriber, null);

		return (null, maxAllowedCopies: maxAllowedCopies);
	}
}
