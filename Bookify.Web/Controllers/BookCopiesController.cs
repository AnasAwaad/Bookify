using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Archive)]
public class BookCopiesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public BookCopiesController(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        var model = _unitOfWork.BookCopies.GetById(id);
        if (model == null)
        {
            return BadRequest();
        }
        model.IsActive = !model.IsActive;
        model.LastUpdatedOn = DateTime.Now;
        model.LastUpdatedById = User.GetUserId();

        _unitOfWork.SaveChanges();

        return Ok();
    }

    [AjaxOnly]
    public IActionResult Create(int bookId)
    {
        var book = _unitOfWork.Books.GetById(bookId);
        if (book is null) return NotFound();

        BookCopyFormViewModel viewModel = new()
        {
            BookId = bookId,
            IsBookAvailableForRental = book.IsAvailableForRental && book.IsAvailableForRental,
        };
        return PartialView("Form", viewModel);
    }

    [HttpPost]
    public IActionResult Create(BookCopyFormViewModel model)
    {
        var book = _unitOfWork.Books.GetById(model.BookId);
        if (book is null) return NotFound();
        var bookCopy = new BookCopy()
        {
            IsAvailableForRental = model.IsAvailableForRental,
            EditionNumber = model.EditionNumber,
            CreatedById = User.GetUserId(),
            CreatedOn = DateTime.Now
        };

        book.BookCopies.Add(bookCopy);

        _unitOfWork.SaveChanges();

        var bookcopyViewModel = _mapper.Map<BookCopyViewModel>(bookCopy);
        return PartialView("_BookCopyRow", bookcopyViewModel);
    }

    [AjaxOnly]
    public IActionResult Update(int id)
    {
        var model = _unitOfWork.BookCopies.Find(b => b.Id == id, b => b.Include(c => c.Book)!);
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
    public IActionResult Update(BookCopyFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var bookCopy = _unitOfWork.BookCopies.Find(b => b.Id == model.Id);

        if (bookCopy == null)
        {
            return BadRequest();
        }
        bookCopy.EditionNumber = model.EditionNumber;
        bookCopy.LastUpdatedOn = DateTime.Now;
        bookCopy.IsAvailableForRental = model.IsAvailableForRental;
        bookCopy.LastUpdatedById = User.GetUserId();

        _unitOfWork.SaveChanges();

        var viewModel = _mapper.Map<BookCopyViewModel>(bookCopy);

        return PartialView("_BookCopyRow", viewModel);
    }
}
