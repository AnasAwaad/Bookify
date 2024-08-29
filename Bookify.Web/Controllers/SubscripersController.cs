using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Reception)]
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
		return View("Form",viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Create(SubscriperFormViewModel viewModel)
	{
		if (!ModelState.IsValid)
			return View(PopulateViewModel());

		var model = _mapper.Map<Subscriper>(viewModel);

		var imageName = $"{Guid.NewGuid()}{Path.GetExtension(viewModel.Image!.FileName)}";

		(bool isUploaded, string? errorMessage) = _imageService.UploadImage(viewModel.Image, imageName, "images\\subscripers", true);
		if (!isUploaded)
		{
			ModelState.AddModelError("Image", errorMessage!);
			return View(PopulateViewModel(viewModel));
		}
		model.ImageUrl = "/images/subscripers/" + imageName;
		model.ImageThumbnailUrl = "/images/subscripers/thumb/" + imageName;

		model.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
		model.CreatedOn = DateTime.Now;

		_context.Subscripers.Add(model);
		_context.SaveChanges();
		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public IActionResult Update(int id)
	{
		var subscriper = _context.Subscripers.Find(id);
		if (subscriper is null)
			return NotFound();
		var viewModel = PopulateViewModel(_mapper.Map<SubscriperFormViewModel>(subscriper));
		return View("Form", viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Update(SubscriperFormViewModel viewModel)
	{
		if (!ModelState.IsValid)
			return View("Form", PopulateViewModel(viewModel));

		var model = _context.Subscripers.Find(viewModel.Id);

		if (model is null) return NotFound();


		if (viewModel.Image is not null)
		{
			var imageName = $"{Guid.NewGuid()}{Path.GetExtension(viewModel.Image.FileName)}";
			if(!string.IsNullOrEmpty(model.ImageUrl))
				_imageService.DeleteImage(model.ImageUrl, model.ImageThumbnailUrl);

			(bool isUploaded, string? errorMessage) = _imageService.UploadImage(viewModel.Image, imageName, "images\\subscripers", true);
			if (!isUploaded)
			{
				ModelState.AddModelError("Image", errorMessage!);
				return View("Form", PopulateViewModel(viewModel));
			}
			viewModel.ImageUrl = "/images/subscripers/" + imageName;
			viewModel.ImageThumbnailUrl = "/images/subscripers/thumb/" + imageName;
		}
		else
		{
			viewModel.ImageUrl = model.ImageUrl;
			viewModel.ImageThumbnailUrl = model.ImageThumbnailUrl;
		}

		model = _mapper.Map(viewModel, model);
		model.LastUpdatedOn = DateTime.Now;
		model.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

		_context.SaveChanges();
		return RedirectToAction(nameof(Index));
	}


	[HttpPost]
	public IActionResult IsAllowedEmail(SubscriperFormViewModel model)
	{
		var subscriper = _context.Subscripers.Where(s => s.Email == model.Email).FirstOrDefault();
		var isAllowed = subscriper is null || subscriper.Id == model.Id;
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
		var areas = _context.Areas.Where(a => a.CityId == cityId).Select(a => new { Id = a.Id, Text = a.Name }).ToList();
		return Json(new { areas });
	}

	private SubscriperFormViewModel PopulateViewModel(SubscriperFormViewModel? model = null)
	{
		var viewModel = model is null ? new SubscriperFormViewModel() : model;
		var cities = _context.Cities.Where(c => c.IsActive);
		viewModel.Cities = _mapper.Map<IEnumerable<SelectListItem>>(cities);

		if (model?.CityId > 0)
		{
			var areas = _context.Areas
					 .Include(a => a.City)
					 .Where(a => a.City.Id == viewModel.CityId && a.IsActive)
					 .ToList();
			viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
		}

		return viewModel;
	}
}
