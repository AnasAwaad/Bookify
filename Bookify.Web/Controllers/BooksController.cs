using Bookify.Application.Common.Services.Authors;
using Bookify.Application.Common.Services.Books;
using Bookify.Application.Common.Services.Categories;
using Bookify.Domain.Dtos;
using Bookify.Domain.Entities;
using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Archive)]
public class BooksController : Controller
{
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private readonly IBookService _bookService;
    private readonly IAuthorService _authorService;
    private readonly ICategoryService _categoryService;

    public BooksController(IMapper mapper, IImageService imageService, IWebHostEnvironment webHostEnvironment, IBookService bookService, IAuthorService authorService, ICategoryService categoryService)
    {

        _mapper = mapper;
        _imageService = imageService;
        _webHostEnvironment = webHostEnvironment;
        _bookService = bookService;
        _authorService = authorService;
        _categoryService = categoryService;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Details(int id)
    {
        var query = _bookService.GetDetails();
            
        var viewModel = _mapper.ProjectTo<BookViewModel>(query).SingleOrDefault(b => b.Id == id);

        if (viewModel is null) 
            return NotFound();

        return View(viewModel);
    }


    public IActionResult Create()
    {
        return View("Form", PopulateBookVM());
    }

    [HttpPost]
    public IActionResult Create(BookFormViewModel model)
    {

        if (!ModelState.IsValid)
            return View("Form", PopulateBookVM());

        var book = _mapper.Map<Book>(model);

        
        if (model.ImageFile is not null)
        {
            string imageName = Guid.NewGuid() + Path.GetExtension(model.ImageFile!.FileName);

            var result = _imageService.UploadImage(model.ImageFile!, imageName, "images\\books", hasThumbinal: true);

            if (result.isUploaded)
            {
                book.ImageUrl = $"/images/books/{imageName}";
                book.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";
            }
            else
            {
                ModelState.AddModelError("ImageFile", result.errorMessage!);
                return View("Form", PopulateBookVM(model));
            }
        }

        book=_bookService.Add(book, model.SelectedCategories, User.GetUserId());
        
        return RedirectToAction(nameof(Details), new {id=book.Id});
    }


    public IActionResult Update(int id)
    {

        var book = _bookService.GetWithCategories(id);
        if (book is null)
            return NotFound();

        var bookVM = _mapper.Map<BookFormViewModel>(book);
        bookVM.SelectedCategories = book.Categories.Select(x => x.CategoryId).ToList();
        return View("Form", PopulateBookVM(bookVM));
    }

    [HttpPost]
    public IActionResult Update(BookFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateBookVM(model));


        var book = _bookService.GetWithCategories(model.Id);

        if (book is null)
            return NotFound();

        if (model.ImageFile is not null)
        {
            if (!string.IsNullOrEmpty(book.ImageUrl))
            {
                _imageService.DeleteImage(book.ImageUrl, book.ImageThumbnailUrl);
            }

            string imageName = Guid.NewGuid() + Path.GetExtension(model.ImageFile!.FileName);

            var result = _imageService.UploadImage(model.ImageFile!, imageName, "images\\books", hasThumbinal: true);

            if (result.isUploaded)
            {
                model.ImageUrl = $"/images/books/{imageName}";
                model.ImageThumbnailUrl = $"/images/books/thumb/{imageName}";
            }
            else
            {
                ModelState.AddModelError("ImageFile", result.errorMessage!);
                return View("Form", PopulateBookVM(model));
            }

        }
        else if (!string.IsNullOrEmpty(book.ImageUrl))
        {
            model.ImageUrl = book.ImageUrl;
            model.ImageThumbnailUrl = book.ImageThumbnailUrl;
        }
        book = _mapper.Map(model, book);

        book=_bookService.Update(book,model.SelectedCategories,User.GetUserId());

        return RedirectToAction(nameof(Details), new {id=book.Id});
    }


    private BookFormViewModel PopulateBookVM(BookFormViewModel? model = null)
    {

        var bookVM = model is null ? new BookFormViewModel() : model;

        var authors = _authorService.GetActiveAuthors();
        var categories = _categoryService.GetActiveCategories();

        bookVM.AuthorSelectList = _mapper.Map<IEnumerable<SelectListItem>>(authors);
        bookVM.CategorySelectList = _mapper.Map<IEnumerable<SelectListItem>>(categories);
        return bookVM;
    }

    public IActionResult IsBookAllowed(BookFormViewModel model)
    {
        return Json(_bookService.IsBookAllowed(model.Id, model.Title, model.AuthorId));
    }

    #region Ajax Request Handles

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult GetBooks()
    {
        var skip = Convert.ToInt32(Request.Form["start"]);
        var pageSize = Convert.ToInt32(Request.Form["length"]);
        var orderColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"]);
        var orderColumnName = Request.Form[$"columns[{orderColumnIndex}][name]"];
        var orderColumnDirection = Request.Form["order[0][dir]"];
        var searchValue = Request.Form["search[value]"];

        var filteredDto=new FilteredDto(skip,pageSize,orderColumnIndex,orderColumnName!,orderColumnDirection!,searchValue!);

        var (books, count) = _bookService.GetFiltered(filteredDto);

        var booksVM = _mapper.ProjectTo<BookRowViewModel>(books).ToList();

        return Json(new { recordsFiltered = count, recordsTotal=count, data = booksVM });
    }


    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        
        var book = _bookService.ToggleStatus(id, User.GetUserId());
        if (book is null)
            return NotFound();
        return Ok();
    }
    #endregion


}
