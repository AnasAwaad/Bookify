using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Dynamic.Core;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Archive)]
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

    private readonly IList<string> _allowedExtensions = new List<string>() { ".png", ".jpg", ".jpeg" };
    private readonly int _allowedSize = 1048576;

    private readonly IWebHostEnvironment _webHostEnvironment;
    public BooksController(ApplicationDbContext context, IMapper mapper, IImageService imageService, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _mapper = mapper;
        _imageService = imageService;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }


    public IActionResult Details(int id)
    {
        var book = _context.Books
            .Include(b => b.BookCopies)
            .Include(b => b.Author)
            .Include(b => b.Categories)
            .ThenInclude(b => b.Category)
            .SingleOrDefault(b => b.Id == id);
        if (book is null)
            return NotFound();

        var viewModel = _mapper.Map<BookViewModel>(book);
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

        // add categories for new book 
        foreach (var category in model.SelectedCategories)
            book.Categories.Add(new BookCategory() { CategoryId = category });

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


        book.CreatedOn = DateTime.Now;
        book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        _context.Books.Add(book);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }


    public IActionResult Update(int id)
    {
        var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == id);

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


        var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == model.Id);
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

        book.LastUpdatedOn = DateTime.Now;
        book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        book.Categories = new List<BookCategory>();

        foreach (var category in model.SelectedCategories)
            book.Categories.Add(new BookCategory { CategoryId = category });


        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }


    private BookFormViewModel PopulateBookVM(BookFormViewModel? model = null)
    {

        var bookVM = model is null ? new BookFormViewModel() : model;

        var authors = _context.Authors.Where(a => a.IsActive).OrderBy(a => a.Name).ToList();
        var categories = _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToList();

        bookVM.AuthorSelectList = _mapper.Map<IEnumerable<SelectListItem>>(authors);
        bookVM.CategorySelectList = _mapper.Map<IEnumerable<SelectListItem>>(categories);
        return bookVM;
    }

    [HttpPost]
    public IActionResult IsBookAllowed(BookFormViewModel model)
    {

        var book = _context.Books.SingleOrDefault(c => c.Title == model.Title && c.AuthorId == model.AuthorId);

        if (book == null || book.Id == model.Id)
            return Json(true);
        return Json(false);
    }

    #region Ajax Request Handles

    [HttpPost]
    public IActionResult GetBooks()
    {
        var skip = Convert.ToInt32(Request.Form["start"]);
        var pageSize = Convert.ToInt32(Request.Form["length"]);
        var orderColumnIndex = Convert.ToInt32(Request.Form["order[0][column]"]);
        var orderColumnName = Request.Form[$"columns[{orderColumnIndex}][name]"];
        var orderColumnDirection = Request.Form["order[0][dir]"];
        var searchValue = Request.Form["search[value]"];


        var allbooks = _context.Books.ToList();
        IQueryable<Book> books = _context.Books.Include(b => b.Author).Include(b => b.Categories).ThenInclude(b => b.Category);

        if (!string.IsNullOrEmpty(searchValue))
            books = books.Where(b => b.Title.Contains(searchValue!) || b.Author!.Name.Contains(searchValue!));

        books = books.OrderBy($"{orderColumnName} {orderColumnDirection}");  //orderBy from system.Linq.Dynamic lib

        var data = books.Skip(skip).Take(pageSize).ToList();
        var booksVM = _mapper.Map<IEnumerable<BookViewModel>>(data);
        var recordsTotal = books.Count();

        return Json(new { recordsFiltered = recordsTotal, recordsTotal, data = booksVM });
    }


    [ValidateAntiForgeryToken]
    public IActionResult ToggleStatus(int id)
    {
        var book = _context.Books.Find(id);
        if (book == null)
        {
            return NotFound();
        }
        book.LastUpdatedOn = DateTime.Now;
        book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        book.IsActive = !book.IsActive;
        _context.SaveChanges();
        return Ok();
    }
    #endregion


}
