using Microsoft.AspNetCore.Mvc;
using System.Linq.Dynamic.Core;

namespace Bookify.Web.Controllers;
public class DashboardController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;

	public DashboardController(ApplicationDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public IActionResult Index()
	{
		var numberOfBooks=_context.Books.Count(b=>b.IsActive);
		var numberOfSubscripers=_context.Subscripers.Count(b=>b.IsActive);
		var latestBooks = _context.Books.Include(b=>b.Author).OrderByDescending(b => b.CreatedOn).Take(8).ToList();

		var topBooks = _context.RentalCopies
					.Include(r => r.BookCopy)
					.ThenInclude(c => c!.Book)
					.ThenInclude(b => b!.Author)
					.GroupBy(r => new
					{
						r.BookCopy!.BookId,
						r.BookCopy!.Book!.Title,
						r.BookCopy!.Book!.ImageThumbnailUrl,
						AuthorName = r.BookCopy!.Book!.Author!.Name,

					})
					.Select(b => new
					{
						b.Key.BookId,
						b.Key.AuthorName,
						b.Key.Title,
						b.Key.ImageThumbnailUrl,
						Count = b.Count()
					})
					.OrderByDescending(b => b.Count)
					.Take(6)
					.Select(b=>new BookViewModel
					{
						Id=b.BookId,
						AuthorName=b.AuthorName,
						Title = b.Title,
						ImageThumbnailUrl=b.ImageThumbnailUrl
					})
					.ToList();


		DashboardViewModel viewModel = new()
		{
			NumberOfBooks = numberOfBooks,
			NumberOfSubscriber = numberOfSubscripers,
			LatestBooks=_mapper.Map<IEnumerable<BookViewModel>>(latestBooks),
			TopBooks=topBooks
		};
		return View(viewModel);
	}

	[AjaxOnly]
	public IActionResult GetNumberOfRentalsPerDay(DateTime? startDate,DateTime? endDate)
	{
		startDate ??= DateTime.Today.AddDays(-29);
		endDate ??= DateTime.Today;

		var rentals = _context.RentalCopies
			.Where(r=>r.RentalDate >= startDate && r.RentalDate <= endDate)
			.GroupBy(r => new{ r.RentalDate })
			.Select(r => new
			{
				text=r.Key.RentalDate.ToString("d MMM"),
				value=r.Count().ToString()
			}).ToList();

		return Ok(rentals);
	}
}
