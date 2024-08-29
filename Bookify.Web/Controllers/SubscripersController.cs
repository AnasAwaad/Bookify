using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Bookify.Web.Controllers;
[Authorize(Roles =AppRoles.Reception)]
public class SubscripersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;

	public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService)
	{
		_context = context;
		_mapper = mapper;
		_imageService = imageService;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SubscriperFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.Cities = _context.Cities.Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
			return View(viewModel);
        }

        var model = _mapper.Map<Subscriper>(viewModel);
        if(viewModel.Image is not null)
        {
            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(viewModel.Image!.FileName)}";

            (bool isUploaded,string?errorMessage)=_imageService.UploadImage(viewModel.Image, imageName, "images\\subscripers", true);
            if (!isUploaded)
            {
                ModelState.AddModelError("Image", errorMessage!);
				viewModel.Cities = _context.Cities.Select(c => new SelectListItem()
				{
					Value = c.Id.ToString(),
					Text = c.Name
				});
				return View(viewModel);
            }
            model.ImageUrl = "/images/subscripers/"+imageName;
            model.ImageThumbnailUrl= "/images/subscripers/thumb/" + imageName;
		}

        model.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        model.CreatedOn = DateTime.Now;

        _context.Subscripers.Add(model);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }



    [HttpPost]
    public IActionResult IsAllowedEmail(SubscriperFormViewModel model)
    {
        var subscriper = _context.Subscripers.Where(s => s.Email == model.Email).FirstOrDefault();
        var isAllowed=subscriper is null  || subscriper.Id == model.Id;
        return Ok(isAllowed);
    }

	[HttpPost]
	public IActionResult IsAllowedMobileNumber(SubscriperFormViewModel model)
	{
		var subscriper = _context.Subscripers.Where(s => s.MobileNumber == model.MobileNumber).FirstOrDefault();
		var isAllowed = subscriper is null || subscriper.Id == model.Id;
		return Ok(isAllowed);
	}

	[HttpPost]
	public IActionResult IsAllowedNationalId(SubscriperFormViewModel model)
	{
		var subscriper = _context.Subscripers.Where(s => s.NationalId == model.NationalId).FirstOrDefault();
		var isAllowed = subscriper is null || subscriper.Id == model.Id;
		return Ok(isAllowed);
	}
	[AjaxOnly]
    public IActionResult GetAreasBasedOnCity(int cityId)
    {
        var areas=_context.Areas.Where(a=>a.CityId== cityId).Select(a => new {Id=a.Id,Text=a.Name}).ToList();
        return Json(new { areas });
    }
}
