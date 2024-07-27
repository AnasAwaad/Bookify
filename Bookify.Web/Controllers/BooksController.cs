using Bookify.Web.Core.Consts;
using Bookify.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers;
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
	private readonly IList<string> _allowedExtensions = new List<string>() { ".png", ".jpg",".jpeg" };
	private readonly int _allowedSize = 1048576;
	private readonly IWebHostEnvironment _webHostEnvironment;
	public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
	{
		_context = context;
		_mapper = mapper;
		_webHostEnvironment = webHostEnvironment;
	}

	public IActionResult Index()
    {
        return View();
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
		// check extension of image
		if (!_allowedExtensions.Contains(Path.GetExtension(model.ImageFile.FileName).ToLower()))
		{
			ModelState.AddModelError("ImageFile", Errors.AllowedExtensions);
			return View("Form", PopulateBookVM(model));
		}
		// check size of image
		if(model.ImageFile.Length > _allowedSize)
		{
			ModelState.AddModelError("ImageFile", Errors.AllowedSize);
			return View("Form", PopulateBookVM(model));
		}
		
		var book = _mapper.Map<Book>(model);

		// add categories for new book 
		foreach (var category in model.SelectedCategories)
			book.Categories.Add(new BookCategory() { CategoryId = category});



		book.ImageUrl = Uploader.UploadFile(model.ImageFile,"Book",_webHostEnvironment);
		_context.Books.Add(book);
		_context.SaveChanges();
		return RedirectToAction("Index");
	}



	private BookFormViewModel PopulateBookVM(BookFormViewModel? model=null)
	{
		
		var bookVM = model is null ? new BookFormViewModel() : model;

		var authors = _context.Authors.Where(a => a.IsActive).OrderBy(a=>a.Name).ToList();
		var categories = _context.Categories.Where(c => c.IsActive).OrderBy(c=>c.Name).ToList();

		bookVM.AuthorSelectList = _mapper.Map<IEnumerable<SelectListItem>>(authors);
		bookVM.CategorySelectList = _mapper.Map<IEnumerable<SelectListItem>>(categories);
		return bookVM;
	}

	
}
