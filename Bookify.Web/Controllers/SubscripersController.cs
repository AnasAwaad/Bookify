using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using Hangfire;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using System.ComponentModel;
using System.Text.Encodings.Web;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;
namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Reception)]
public class SubscripersController : Controller
{
	private readonly ApplicationDbContext _context;
	private readonly IMapper _mapper;
	private readonly IImageService _imageService;
	private readonly IDataProtector _dataProtector;
	private readonly IWhatsAppClient _whatsAppClient;
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly IEmailBodyBuilder _emailBodyBuilder;
	private readonly IEmailSender _emailSender;

	public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtector, IWhatsAppClient whatsAppClient, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder,IEmailSender emailSender)
	{
		_context = context;
		_mapper = mapper;
		_imageService = imageService;
		_dataProtector = dataProtector.CreateProtector("MySecureKey");
		_whatsAppClient = whatsAppClient;
		_webHostEnvironment = webHostEnvironment;
		_emailBodyBuilder = emailBodyBuilder;
		_emailSender = emailSender;
	}

	public IActionResult Index()
	{
		return View();
	}
	[HttpGet]
	public IActionResult Create()
	{
		return View("Form", PopulateViewModel());
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

		var subscription = new Subscription
		{
			CreatedById = model.CreatedById,
			CreatedOn = model.CreatedOn,
			StartDate = DateTime.Today,
			EndDate = DateTime.Today.AddYears(1),
		};

		model.Subscriptions.Add(subscription);

		_context.Subscripers.Add(model);
		_context.SaveChanges();

		// send welcome message to email user
		var placeholders = new Dictionary<string, string>()
		{
			{"imageUrl","https://res.cloudinary.com/dygrlijla/image/upload/v1723914644/93ae73da-0ad6-4e76-ad40-3ab67cf4c6f1.png" },
			{"imageLogo","https://res.cloudinary.com/dygrlijla/image/upload/v1723914720/logo_nh8slr.png" },
			{"header", $"Hey {model.FirstName}" },
			{"body","Thanks for joining us" },
		};


		var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

		BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(model.Email, "Welcome to bookify", body));

		// send welcome message to whatsapp user
		if (model.HasWhatsApp)
		{
			var component = new List<WhatsAppComponent>()
			{
				new WhatsAppComponent
				{
					Type="body",
					Parameters=new List<object>
					{
						new WhatsAppTextParameter
						{
							Text=User.FindFirst(ClaimTypes.GivenName)!.Value
						}
					}
				}
			};

			var mobileNumber = _webHostEnvironment.IsDevelopment() ? "201067873327" : $"2{model.MobileNumber}";

			BackgroundJob.Enqueue(() => _whatsAppClient.SendMessage(mobileNumber, WhatsAppLanguageCode.English, WhatsAppTemplates.WelcomeMessage, component));


		}
		var subscriperId = _dataProtector.Protect(model.Id.ToString());

		return RedirectToAction(nameof(Details), new { Id = subscriperId });
	}

	public IActionResult Details(string Id)
	{
		var subscriperId = int.Parse(_dataProtector.Unprotect(Id));

		var subscriper = _context.Subscripers.Include(s => s.Area).Include(s => s.City).Include(s=>s.Subscriptions).Include(s=>s.Rentals).ThenInclude(r=>r.RentalCopies).FirstOrDefault(s => s.Id == subscriperId);

		if (subscriper is null)
			return NotFound();
		var viewModel = _mapper.Map<SubscriberViewModel>(subscriper);
		viewModel.Key = Id;

		return View(viewModel);
	}

	[HttpGet]
	public IActionResult Update(string id)
	{
		var subscriperId = int.Parse(_dataProtector.Unprotect(id));

		var subscriper = _context.Subscripers.Find(subscriperId);
		if (subscriper is null)
			return NotFound();
		var viewModel = PopulateViewModel(_mapper.Map<SubscriperFormViewModel>(subscriper));

		viewModel.Key = id;

		return View("Form", viewModel);
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult Update(SubscriperFormViewModel viewModel)
	{
		if (!ModelState.IsValid)
			return View("Form", PopulateViewModel(viewModel));

		var subscriperId = int.Parse(_dataProtector.Unprotect(viewModel.Key!));

		var model = _context.Subscripers.Find(subscriperId);

		if (model is null) return NotFound();


		if (viewModel.Image is not null)
		{
			var imageName = $"{Guid.NewGuid()}{Path.GetExtension(viewModel.Image.FileName)}";
			if (!string.IsNullOrEmpty(model.ImageUrl))
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


		return RedirectToAction(nameof(Details), new { Id = viewModel.Key });
	}


	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult SearchForSubscriper(SubscriperSearchFormViewModel model)
	{
		if (!ModelState.IsValid) return BadRequest();
		var subscriper = _context.Subscripers.SingleOrDefault(s => s.MobileNumber == model.Value || s.Email == model.Value || s.NationalId == model.Value);

		var viewModel = _mapper.Map<SubscriperSearchResultViewModel>(subscriper);

		if (subscriper is not null)
			viewModel.Key = _dataProtector.Protect(subscriper.Id.ToString());

		return PartialView("_SubscriperSearchResult", viewModel);
	}


	[HttpPost]
	public IActionResult IsAllowedEmail(SubscriperFormViewModel model)
	{
		var subscriperId = 0;

		if (!string.IsNullOrEmpty(model.Key))
			subscriperId = int.Parse(_dataProtector.Unprotect(model.Key));

		var subscriper = _context.Subscripers.Where(s => s.Email == model.Email).FirstOrDefault();
		var isAllowed = subscriper is null || subscriper.Id.Equals(subscriperId);

		return Json(isAllowed);
	}

	[HttpPost]
	public IActionResult IsAllowedMobileNumber(SubscriperFormViewModel model)
	{
		var subscriperId = 0;

		if (!string.IsNullOrEmpty(model.Key))
			subscriperId = int.Parse(_dataProtector.Unprotect(model.Key));

		var subscriper = _context.Subscripers.Where(s => s.MobileNumber == model.MobileNumber).FirstOrDefault();
		var isAllowed = subscriper is null || subscriper.Id.Equals(subscriperId);

		return Json(isAllowed);
	}

	[HttpPost]
	public IActionResult IsAllowedNationalId(SubscriperFormViewModel model)
	{

		var subscriperId = 0;

		if (!string.IsNullOrEmpty(model.Key))
			subscriperId = int.Parse(_dataProtector.Unprotect(model.Key));

		var subscriper = _context.Subscripers.Where(s => s.NationalId == model.NationalId).FirstOrDefault();
		var isAllowed = subscriper is null || subscriper.Id.Equals(subscriperId);

		return Json(isAllowed);

	}
	[AjaxOnly]
	public IActionResult GetAreasBasedOnCity(int cityId)
	{
		var areas = _context.Areas.Where(a => a.CityId == cityId).Select(a => new { Id = a.Id, Text = a.Name }).ToList();
		return Json(new { areas });
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public IActionResult renewSubscription(string subscriperKey)
	{
		var subscriberId=int.Parse(_dataProtector.Unprotect(subscriperKey));
		var subscriber = _context.Subscripers.Include(s=>s.Subscriptions).SingleOrDefault(s => s.Id == subscriberId);

		if (subscriber is null)
			return NotFound();
		if (subscriber.IsBlackListed)
			return BadRequest();

		var lastSubscription = subscriber.Subscriptions.Last();
		var startDate = lastSubscription.EndDate > DateTime.Today ? lastSubscription.EndDate.AddDays(1):DateTime.Today ;
		var newSubscription = new Subscription()
		{
			CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
			CreatedOn = DateTime.Now,
			StartDate = startDate,
			EndDate = startDate.AddYears(1)
		};
		subscriber.Subscriptions.Add(newSubscription);
		_context.SaveChanges();

		if (subscriber.HasWhatsApp)
		{
			var component = new List<WhatsAppComponent>()
			{
				new WhatsAppComponent
				{
					Type="body",
					Parameters=new List<object>
					{
						new WhatsAppTextParameter
						{
							Text=subscriber.FirstName
						}
					}
				}
			};

			var mobileNumber = _webHostEnvironment.IsDevelopment() ? "201067873327" : $"2{subscriber.MobileNumber}";

			BackgroundJob.Enqueue(() => _whatsAppClient.SendMessage(mobileNumber, WhatsAppLanguageCode.English, WhatsAppTemplates.WelcomeMessage, component));
		}
		return PartialView("_SubscriptionRow",_mapper.Map<SubscriptionViewModel>(newSubscription	));
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
