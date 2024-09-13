using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Archive)]
public class AuthorsController : Controller
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthorsController(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        var authors = _context.Authors.AsNoTracking().ToList();
        var authorsVM = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
        return View(authorsVM);
    }

    [AjaxOnly]
    public IActionResult Create()
    {
        return PartialView("_UpsertForm");
    }


    [HttpPost]
    public IActionResult Create(UpsertAuthorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return NotFound();
        }

        var author = _mapper.Map<Author>(model);
        author.CreatedById = User.GetUserId();
        author.CreatedOn = DateTime.Now;

        _context.Authors.Add(author);
        _context.SaveChanges();

        var authorVM = _mapper.Map<AuthorViewModel>(author);
        return PartialView("_AuthorRow", authorVM);
    }

    [AjaxOnly]
    public IActionResult Update(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }
        var author = _context.Authors.Find(id);
        if (author == null)
        {
            return NotFound();
        }

        var authorVM = _mapper.Map<UpsertAuthorViewModel>(author);

        return PartialView("_UpsertForm", authorVM);
    }


    [HttpPost]
    public IActionResult Update(UpsertAuthorViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("_UpsertForm", model);
        }
        var author = _context.Authors.Find(model.Id);

        if (author == null)
            return NotFound();

        author = _mapper.Map(model, author);

        author.LastUpdatedOn = DateTime.Now;
        author.LastUpdatedById = User.GetUserId();

        _context.SaveChanges();
        return PartialView("_AuthorRow", _mapper.Map<AuthorViewModel>(author));
    }


    public IActionResult IsAuthorAllowed(UpsertAuthorViewModel model)
    {
        var author = _context.Authors.SingleOrDefault(c => c.Name == model.Name);
        // for new category null 
        // for update category without change the name => category will be filled 
        // check for id of the category with the same name equal model.Id
        if (author == null || author.Id == model.Id)
            return Json(true);
        return Json(false);
    }



    #region Ajax Request Handles

    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        var author = _context.Authors.Find(id);
        if (author == null)
        {
            return NotFound();
        }

        author.LastUpdatedOn = DateTime.Now;
        author.IsActive = !author.IsActive;
        author.LastUpdatedById = User.GetUserId();

        _context.SaveChanges();
        return Json(new { lastUpdatedOn = author.LastUpdatedOn.ToString() });
    }
    #endregion
}
