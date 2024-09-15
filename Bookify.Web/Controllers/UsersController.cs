using Bookify.Application.Common.Services.Auth;
using Bookify.Domain.Consts;
using Bookify.Domain.Dtos;
using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Admin)]
public class UsersController : Controller
{
    private readonly IAuthService _authService;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailBodyBuilder _emailBodyBuilder;
    private readonly IMapper _mapper;

	public UsersController(IMapper mapper, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder, IAuthService authService)
	{
		_mapper = mapper;
		_emailSender = emailSender;
		_webHostEnvironment = webHostEnvironment;
		_emailBodyBuilder = emailBodyBuilder;
		_authService = authService;
	}

	public async Task<IActionResult> Index()
    {
        var users = await _authService.GetUsersAsync();
        List<UserViewModel> viewModel = new List<UserViewModel>();
        foreach (var user in users)
        {
            var item = _mapper.Map<UserViewModel>(user);
            item.IsLockedOut = await _authService.IsLockedOutAsync(user);
            viewModel.Add(item);
        }
        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        var roles = await _authService.GetRolesAsync();
        var viewModel = new UserFormViewModel
        {
            RolesSelectList = roles.Select(r => new SelectListItem(){ Value = r.Name, Text = r.Name })
        };
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var dto = _mapper.Map<CreateUserDto>(model);
        

        var (user,isSucceeded,code,errors) = await _authService.CreateAsync(dto,User.GetUserId());

        if (isSucceeded)
        {
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code!));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user!.Id, code },
                protocol: Request.Scheme);

            var placeholders = new Dictionary<string, string>()
            {
                {"imageUrl","https://res.cloudinary.com/dygrlijla/image/upload/v1723914644/93ae73da-0ad6-4e76-ad40-3ab67cf4c6f1.png" },
                {"imageLogo","https://res.cloudinary.com/dygrlijla/image/upload/v1723914720/logo_nh8slr.png" },
                {"header", $"Hey {user.FullName}, thanks for joining up!" },
                {"body","Please confirm your email" },
                {"url",$"{HtmlEncoder.Default.Encode(callbackUrl!)}" },
                {"linkTitle", "Active Account!" },
            };

            var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Email, placeholders);

            await _emailSender.SendEmailAsync(user.Email!, "Confirm your email", body);

            return PartialView("_UserRow", _mapper.Map<UserViewModel>(user));
        }
        return BadRequest(errors);

    }

    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> Update(string id)
    {
        var user = await _authService.GetUsersByIdAsync(id);
        if (user is null)
            return NotFound();
        var viewModel = _mapper.Map<UserFormViewModel>(user);
        var roles = await _authService.GetRolesAsync();

		viewModel.RolesSelectList = roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        });

        viewModel.SelectedRoles = await _authService.GetUsersRolesAsync(user);
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Update(UserFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = await _authService.GetUsersByIdAsync(model.Id!);

        if (user is null)
            return NotFound();

        user = _mapper.Map(model, user);

        var (isSucceeded, updatedUser, errors) =await _authService.UpdateAsync(user, User.GetUserId(),model.SelectedRoles);

        if (isSucceeded)
        {
			return PartialView("_UserRow", _mapper.Map<UserViewModel>(updatedUser));
		}
		
		return BadRequest(errors);
    }

    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _authService.GetUsersByIdAsync(id);

        if (user is null)
            return BadRequest();

        var viewModel = new UserResetPasswordViewModel() { Id = id };
        return PartialView("_ResetPassword", viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(UserResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        var user = await _authService.GetUsersByIdAsync(model.Id!);

        if (user is null)
            return NotFound();

        var (isSucceeded, resultUser,errors) = await _authService.ResetPasswordAsync(user, User.GetUserId(), model.Password);
        
        if(isSucceeded)
        {
            var viewModel = _mapper.Map<UserViewModel>(resultUser);
            return PartialView("_UserRow", viewModel);
        }

		

		user.PasswordHash = user.PasswordHash;
        await _authService.UpdateAsync(user,User.GetUserId(),null);


        return BadRequest(errors);
    }

    public async Task<IActionResult> AllowUserName(UserFormViewModel model)
    {
        return Json(await _authService.IsAllowedUserName(model.UserName,model.Id!));
    }

    public async Task<IActionResult> AllowEmail(UserFormViewModel model)
    {
		return Json(await _authService.IsAllowedEmail(model.Email, model.Id!));

	}

	[HttpPost]
    public async Task<IActionResult> UnlockUser(string id)
    {
        var user = await _authService.GetUsersByIdAsync(id);

        if (user == null)
            return NotFound();

        var isLocked = await _authService.IsLockedOutAsync(user);
        if (!isLocked)
            return BadRequest("User is Unlock already");

        var isSucceeded = await _authService.SetLockoutEndDateAsync(user, DateTime.Now + TimeSpan.FromMinutes(5));

        return isSucceeded ? Ok() : BadRequest();
    }




    #region Ajax Request Handles

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user=await _authService.ToggleStatus(id, User.GetUserId());

        if (user == null) 
            return NotFound();

        return Json(new { lastUpdatedOn = user.LastUpdatedOn.ToString() });
    }
    #endregion
}
