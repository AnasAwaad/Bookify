using Bookify.Application.Common.Services.BookCopies;
using Bookify.Application.Common.Services.Books;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Archive)]
public class BookCopiesController : Controller
{
    private readonly IBookCopyService _bookCopyService;
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public BookCopiesController(IMapper mapper, IBookCopyService bookCopyService, IBookService bookService)
    {
        _mapper = mapper;
        _bookCopyService = bookCopyService;
        _bookService = bookService;
    }


    [AjaxOnly]
    public IActionResult Create(int bookId)
    {
        var book = _bookService.GetById(bookId);
        if (book is null)
            return NotFound();

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

        var bookCopy = _bookCopyService.Add(model.BookId, model.IsAvailableForRental, model.EditionNumber, User.GetUserId());

        if (bookCopy == null)
            return NotFound();

        var bookcopyViewModel = _mapper.Map<BookCopyViewModel>(bookCopy);
        return PartialView("_BookCopyRow", bookcopyViewModel);
    }

    [AjaxOnly]
    public IActionResult Update(int id)
    {
        var model = _bookCopyService.GetDetails(id);
        if (model == null)
            return BadRequest();

        var viewModel = _mapper.Map<BookCopyFormViewModel>(model);
        return PartialView("Form", viewModel);

    }

    [HttpPost]
    public IActionResult Update(BookCopyFormViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var bookCopy = _bookCopyService.Update(model.Id, model.IsAvailableForRental, model.EditionNumber, User.GetUserId());

        if (bookCopy is null)
            return BadRequest();

        var viewModel = _mapper.Map<BookCopyViewModel>(bookCopy);

        return PartialView("_BookCopyRow", viewModel);
    }

    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        var bookCopy = _bookCopyService.ToggleStatus(id, User.GetUserId());
        if (bookCopy == null)
            return BadRequest();

        return bookCopy is null ? BadRequest() : Ok();
    }
}
