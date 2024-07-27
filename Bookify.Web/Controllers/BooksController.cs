using Bookify.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Controllers;
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
	private readonly IList<string> AllowedExtensions = new List<string>() { ".png", ".jpg" };
	private readonly int AllowedSize = 1048576;
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
        
        return View("Form", CreateBookFormVM());
    }

	[HttpPost]
	public IActionResult Create(BookFormViewModel model)
	{

		if (!ModelState.IsValid)
		{
			return View("Form", CreateBookFormVM());
		}

		if (model.ImageFile is not null && !AllowedExtensions.Contains(Path.GetExtension(model.ImageFile.FileName).ToLower()))
		{
			
			ModelState.AddModelError("ImageFile", "Only .jpg and .png are allowed");
			return View("Form", CreateBookFormVM(model));
		}

		if(model.ImageFile is not null && model.ImageFile.Length > AllowedSize)
		{
			
			ModelState.AddModelError("ImageFile", "Image cannot be more than 1 megabyte!");
			return View("Form", CreateBookFormVM(model));
		}

		var book = _mapper.Map<Book>(model);
		book.ImageUrl = Uploader.UploadFile(model.ImageFile!,"Book",_webHostEnvironment);
		_context.Books.Add(book);
		_context.SaveChanges();
		return RedirectToAction("Index");
	}

	private BookFormViewModel CreateBookFormVM(BookFormViewModel? model=null)
	{
		var bookVM = new BookFormViewModel();
		bookVM.AuthorSelectList = _mapper.Map<IEnumerable<SelectListItem>>(_context.Authors.ToList());
		bookVM.CategorySelectList = _mapper.Map<IEnumerable<SelectListItem>>(_context.Categories.ToList());
		return bookVM;
	}

	
}
