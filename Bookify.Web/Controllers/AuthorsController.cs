using Bookify.Application.Common.Services.Authors;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Archive)]
public class AuthorsController : Controller
{
    private readonly IMapper _mapper;
    private readonly IAuthorService _authorService;

    public AuthorsController(IMapper mapper,IAuthorService authorService)
    {
        _mapper = mapper;
        _authorService = authorService;
    }

    public IActionResult Index()
    {
        var authors = _authorService.GetAll();
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

        var author =_authorService.Add(model.Name, User.GetUserId());

        return PartialView("_AuthorRow", _mapper.Map<AuthorViewModel>(author));
    }

    [AjaxOnly]
    public IActionResult Update(int id)
    {
        var author = _authorService.GetById(id);

        if (author == null)
            return NotFound();

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

        var author=_authorService.Update(model.Id, model.Name, User.GetUserId());

        if (author is null)
            return NotFound();

        return PartialView("_AuthorRow", _mapper.Map<AuthorViewModel>(author));
    }


    public IActionResult IsAuthorAllowed(UpsertAuthorViewModel model)
    {
        var isAllowed = _authorService.IsAuthorAllowed(model.Id, model.Name);
        return Json(isAllowed);
    }

    #region Ajax Request Handles

    [HttpPost]
    public IActionResult ToggleStatus(int id)
    {
        var author=_authorService.ToggleStatus(id, User.GetUserId());

        if (author is null)
            return NotFound();

        return Json(new { lastUpdatedOn = author.LastUpdatedOn.ToString() });
    }
    #endregion
}
