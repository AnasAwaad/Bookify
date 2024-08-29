using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Bookify.Web.Controllers;
[Authorize(Roles =AppRoles.Reception)]
public class SubscripersController : Controller
{
    private readonly ApplicationDbContext _context;

	public SubscripersController(ApplicationDbContext context)
	{
		_context = context;
	}

	public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        SubscriperFormViewModel viewModel = new()
        {
            Cities = _context.Cities.Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
        };
        return View(viewModel);
    }

    public IActionResult GetAreasBasedOnCity(int cityId)
    {
        var areas=_context.Areas.Where(a=>a.CityId== cityId).Select(a => new {Id=a.Id,Text=a.Name}).ToList();
        return Json(new { areas });
    }
}
