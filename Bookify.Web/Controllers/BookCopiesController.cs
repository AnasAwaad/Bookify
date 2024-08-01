using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers;
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
        };
        book.BookCopies.Add(bookCopy);
        _context.SaveChanges();
        var bookcopyViewModel=_mapper.Map<BookCopyViewModel>(bookCopy);
        return PartialView("_BookCopyRow",bookcopyViewModel);
    }

    //public IActionResult Update(int id)
    //{
    //    var model = _context.BookCopies.Find(id);
    //    if (model == null)
    //    {
    //        return BadRequest();
    //    }
        
    //}
}
