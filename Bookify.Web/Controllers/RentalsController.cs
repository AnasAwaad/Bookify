using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;
public class RentalsController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public RentalsController(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public IActionResult Create()
	{
		return View();
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


		var model=_mapper.Map<BookCopyViewModel>(copy);	
		return PartialView("_RentalCopyDetails",model);
	}
}
