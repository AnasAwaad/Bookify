using Bookify.Application.Common.Services.Areas;
using Bookify.Application.Common.Services.Cities;
using Bookify.Application.Common.Services.Subscripers;
using Bookify.Web.Services;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;
namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Reception)]
public class SubscripersController : Controller
{
    private readonly ISubscriperService _subscriperService;
    private readonly IAreaService _areaService;
    private readonly ICityService _cityService;
    private readonly IMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IDataProtector _dataProtector;
    private readonly IWhatsAppClient _whatsAppClient;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailBodyBuilder _emailBodyBuilder;
    private readonly IEmailSender _emailSender;

    public SubscripersController(IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtector, IWhatsAppClient whatsAppClient, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder, IEmailSender emailSender, ISubscriperService subscriperService, IAreaService areaService, ICityService cityService)
    {
        _mapper = mapper;
        _imageService = imageService;
        _dataProtector = dataProtector.CreateProtector("MySecureKey");
        _whatsAppClient = whatsAppClient;
        _webHostEnvironment = webHostEnvironment;
        _emailBodyBuilder = emailBodyBuilder;
        _emailSender = emailSender;
        _subscriperService = subscriperService;
        _areaService = areaService;
        _cityService = cityService;
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

        var subscriper=_subscriperService.AddSubscriper(model,User.GetUserId());

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
        var subscriperId = _dataProtector.Protect(subscriper.Id.ToString());

        return RedirectToAction(nameof(Details), new { Id = subscriperId });
    }

    public IActionResult Details(string Id)
    {
        var subscriperId = int.Parse(_dataProtector.Unprotect(Id));

        var subscriper = _subscriperService.GetDatails();

        var viewModel = _mapper.ProjectTo<SubscriberViewModel>(subscriper).SingleOrDefault(s=>s.Id==subscriperId);

        if (viewModel is null)
            return NotFound();

        viewModel.Key = Id;

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Update(string id)
    {
        var subscriperId = int.Parse(_dataProtector.Unprotect(id));

        var subscriper = _subscriperService.GetById(subscriperId);

        if (subscriper is null)
            return NotFound();
        var viewModel = PopulateViewModel(_mapper.Map<SubscriperFormViewModel>(subscriper));

        viewModel.Key = id;

        return View("Form", viewModel);
    }

    [HttpPost]
    public IActionResult Update(SubscriperFormViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View("Form", PopulateViewModel(viewModel));

        var subscriperId = int.Parse(_dataProtector.Unprotect(viewModel.Key!));

        var subscriper = _subscriperService.GetById(subscriperId);

        if (subscriper is null) return NotFound();


        if (viewModel.Image is not null)
        {
            var imageName = $"{Guid.NewGuid()}{Path.GetExtension(viewModel.Image.FileName)}";
            if (!string.IsNullOrEmpty(subscriper.ImageUrl))
                _imageService.DeleteImage(subscriper.ImageUrl, subscriper.ImageThumbnailUrl);

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
            viewModel.ImageUrl = subscriper.ImageUrl;
            viewModel.ImageThumbnailUrl = subscriper.ImageThumbnailUrl;
        }

        subscriper = _mapper.Map(viewModel, subscriper);

        _subscriperService.Update(subscriper, User.GetUserId());

        return RedirectToAction(nameof(Details), new { Id = viewModel.Key });
    }


    [HttpPost]
    public IActionResult SearchForSubscriper(SearchFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        
        var subscriper=_subscriperService.SearchForSubscriper(model.Value);
        var viewModel = _mapper.Map<SubscriperSearchResultViewModel>(subscriper);

        if (subscriper is not null)
            viewModel.Key = _dataProtector.Protect(subscriper.Id.ToString());

        return PartialView("_SubscriperSearchResult", viewModel);
    }


    public IActionResult IsAllowedEmail(SubscriperFormViewModel model)
    {
        var subscriperId = 0;

        if (!string.IsNullOrEmpty(model.Key))
            subscriperId = int.Parse(_dataProtector.Unprotect(model.Key));

        var isAllowed = _subscriperService.IsAllowedEmail(subscriperId, model.Email);
        return Json(isAllowed);
    }

    public IActionResult IsAllowedMobileNumber(SubscriperFormViewModel model)
    {
        var subscriperId = 0;

        if (!string.IsNullOrEmpty(model.Key))
            subscriperId = int.Parse(_dataProtector.Unprotect(model.Key));

        var isAllowed = _subscriperService.IsAllowedMobileNumber(subscriperId, model.MobileNumber);
        return Json(isAllowed);
    }

    public IActionResult IsAllowedNationalId(SubscriperFormViewModel model)
    {

        var subscriperId = 0;

        if (!string.IsNullOrEmpty(model.Key))
            subscriperId = int.Parse(_dataProtector.Unprotect(model.Key));

        var isAllowed = _subscriperService.IsAllowedNationalId(subscriperId, model.NationalId);
        return Json(isAllowed);
    }
    [AjaxOnly]
    public IActionResult GetAreasBasedOnCity(int cityId)
    {
        //var areas = _context.Areas.Where(a => a.CityId == cityId).Select(a => new { Id = a.Id, Text = a.Name }).ToList();
        var areas = _areaService.GetAreasByCity(cityId);
        return Json(new { areas });
    }

    [HttpPost]
    public IActionResult renewSubscription(string subscriperKey)
    {
        var subscriberId = int.Parse(_dataProtector.Unprotect(subscriperKey));
        var subscriper = _subscriperService.GetSubscriperWithSubscription(subscriberId);

        if (subscriper is null)
            return NotFound();

        if (subscriper.IsBlackListed)
            return BadRequest();

        var newSubscription = _subscriperService.RenewSubscription(subscriberId, User.GetUserId());

        if (subscriper.HasWhatsApp)
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
                            Text=subscriper.FirstName
                        }
                    }
                }
            };

            var mobileNumber = _webHostEnvironment.IsDevelopment() ? "201067873327" : $"2{subscriper.MobileNumber}";

            BackgroundJob.Enqueue(() => _whatsAppClient.SendMessage(mobileNumber, WhatsAppLanguageCode.English, WhatsAppTemplates.WelcomeMessage, component));
        }
        return PartialView("_SubscriptionRow", _mapper.Map<SubscriptionViewModel>(newSubscription));
    }

    private SubscriperFormViewModel PopulateViewModel(SubscriperFormViewModel? model = null)
    {
        var viewModel = model is null ? new SubscriperFormViewModel() : model;
        var cities = _cityService.GetActiveCities();

        viewModel.Cities = _mapper.Map<IEnumerable<SelectListItem>>(cities);

        if (model?.CityId > 0)
        {
            var areas = _areaService.GetAreasByCity(model.CityId);
            viewModel.Areas = _mapper.Map<IEnumerable<SelectListItem>>(areas);
        }

        return viewModel;
    }
}
