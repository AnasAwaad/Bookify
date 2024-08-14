using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookify.Web.Controllers;
[Authorize(Roles =AppRoles.Archive)]
public class BookCopiesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BookCopiesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ToggleStatus(int id)
    {
        var model = _context.BookCopies.Find(id);
        if (model == null)
        {
            return BadRequest();
        }
        model.IsActive=!model.IsActive;
        model.LastUpdatedOn= DateTime.Now;
		model.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
		_context.SaveChanges();
        return Ok();
    }

    [AjaxOnly]
    public IActionResult Create(int bookId)
    {
        var book=_context.Books.Find(bookId);
        if (book is null) return NotFound();

        BookCopyFormViewModel viewModel =new ()
        { 
            BookId = bookId,
            IsBookAvailableForRental= book.IsAvailableForRental && book.IsAvailableForRental,
        };
        return PartialView("Form",viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(BookCopyFormViewModel model)
    {
        var book = _context.Books.Find(model.BookId);
        if (book is null) return NotFound();
        var bookCopy = new BookCopy()
        {
            IsAvailableForRental = model.IsAvailableForRental,
            EditionNumber = model.EditionNumber,
            CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
            CreatedOn = DateTime.Now
        };

        book.BookCopies.Add(bookCopy);
        _context.SaveChanges();

        var bookcopyViewModel=_mapper.Map<BookCopyViewModel>(bookCopy);
        return PartialView("_BookCopyRow",bookcopyViewModel);
    }

    [AjaxOnly]
    public IActionResult Update(int id)
    {
        var model = _context.BookCopies.Include(b=>b.Book).SingleOrDefault(b=>b.Id==id);
        if (model == null)
        {
            return BadRequest();
        }
        var viewModel = new BookCopyFormViewModel()
        {
            Id = id,
            BookId = model.Id,
            EditionNumber = model.EditionNumber,
            IsAvailableForRental = model.IsAvailableForRental,
            IsBookAvailableForRental = model.Book!.IsAvailableForRental,
        };
        return PartialView("Form", viewModel);

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(BookCopyFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var bookCopy = _context.BookCopies.SingleOrDefault(b => b.Id == model.Id);

        if (bookCopy == null)
        {
            return BadRequest();
        }
        bookCopy.EditionNumber = model.EditionNumber;
        bookCopy.LastUpdatedOn=DateTime.Now;
        bookCopy.IsAvailableForRental=model.IsAvailableForRental;
        bookCopy.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        _context.SaveChanges();

        var viewModel=_mapper.Map<BookCopyViewModel>(bookCopy);

        return PartialView("_BookCopyRow", viewModel);
    }
}
