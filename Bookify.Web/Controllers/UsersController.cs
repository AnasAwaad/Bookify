using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;

namespace Bookify.Web.Controllers;
[Authorize(Roles = AppRoles.Admin)]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMapper _mapper;

    public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
    {
        _userManager = userManager;
        _mapper = mapper;
        _roleManager = roleManager;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {

        



        var users = await _userManager.Users.ToListAsync();
        List<UserViewModel> viewModel = new List<UserViewModel>();
        foreach (var user in users)
        {
            var item = _mapper.Map<UserViewModel>(user);
            item.IsLockedOut = await _userManager.IsLockedOutAsync(user);
            viewModel.Add(item);
        }
        return View(viewModel);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new UserFormViewModel
        {
            RolesSelectList = await _roleManager.Roles.Select(r => new SelectListItem()
            {
                Value = r.Name,
                Text = r.Name
            }).ToListAsync()
        };
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = new ApplicationUser
        {
            FullName = model.FullName,
            UserName = model.UserName,
            Email = model.Email,
            CreatedOn = DateTime.Now,
            CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
            IsActive=true
            
        };

        var result = await _userManager.CreateAsync(user, model.Password!);
        if (result.Succeeded)
        {
            var res = await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code},
                protocol: Request.Scheme);


            var templatePath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";

            StreamReader streamReader = new StreamReader(templatePath);
            var body = streamReader.ReadToEnd();
            streamReader.Close();

            body = body.Replace("[imageUrl]", "https://res.cloudinary.com/dygrlijla/image/upload/v1723914644/93ae73da-0ad6-4e76-ad40-3ab67cf4c6f1.png")
                .Replace("[imageLogo]", "https://res.cloudinary.com/dygrlijla/image/upload/v1723914720/logo_nh8slr.png")
                .Replace("[header]", $"Hey {user.FullName}, thanks for joining up!")
                .Replace("[body]", "Please confirm your email")
                .Replace("[url]", $"{HtmlEncoder.Default.Encode(callbackUrl!)}")
                .Replace("[linkTitle]", "Active Account!");

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",body);




            return PartialView("_UserRow", _mapper.Map<UserViewModel>(user));
        }
        return BadRequest(string.Join(",", result.Errors.Select(e => e.Description)));

    }

    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> Update(string id)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.Id == id);
        if (user is null)
            return NotFound();
        var viewModel = _mapper.Map<UserFormViewModel>(user);
        viewModel.RolesSelectList = await _roleManager.Roles.Select(r => new SelectListItem
        {
            Value = r.Name,
            Text = r.Name
        }).ToListAsync();
        viewModel.SelectedRoles = await _userManager.GetRolesAsync(user);
        return PartialView("_Form", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserFormViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
        if (user is null)
            return NotFound();
        user = _mapper.Map(model, user);

        user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        user.LastUpdatedOn = DateTime.Now;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            var rolesUpdated = !currentRoles.SequenceEqual(model.SelectedRoles);

            if (rolesUpdated)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            }

			await _userManager.UpdateSecurityStampAsync(user);

			return PartialView("_UserRow", _mapper.Map<UserViewModel>(user));
        }
        return BadRequest(string.Join(",", result.Errors.Select(e => e.Description)));
    }

    [HttpGet]
    [AjaxOnly]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return BadRequest();

        var viewModel = new UserResetPasswordViewModel() { Id = id };
        return PartialView("_ResetPassword", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(UserResetPasswordViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest();
        var user = await _userManager.FindByIdAsync(model.Id!);

        if (user is null)
            return NotFound();

        var currentPasswordHash = user.PasswordHash;

        await _userManager.RemovePasswordAsync(user);

        var res = await _userManager.AddPasswordAsync(user, model.Password);

        if (res.Succeeded)
        {
            user.LastUpdatedOn = DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            await _userManager.UpdateAsync(user);

            var viewModel = _mapper.Map<UserViewModel>(user);
            return PartialView("_UserRow", viewModel);
        }

        user.PasswordHash = currentPasswordHash;
        await _userManager.UpdateAsync(user);


        return BadRequest(string.Join(",", res.Errors.Select(e => e.Description)));
    }
    [HttpPost]
    public async Task<IActionResult> AllowUserName(UserFormViewModel model)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == model.UserName);

        var isAllowed = user is null || user.Id == model.Id;
        return Json(isAllowed);
    }

    [HttpPost]
    public async Task<IActionResult> AllowEmail(UserFormViewModel model)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        var isAllowed = user is null || user.Id == model.Id;
        return Json(isAllowed);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var isLocked = await _userManager.IsLockedOutAsync(user);
        if (!isLocked)
        {
            return BadRequest("User is Unlock already");
        }

        var setLockoutEndDateTask = await _userManager.SetLockoutEndDateAsync(user, DateTime.Now - TimeSpan.FromMinutes(1));

        if (setLockoutEndDateTask.Succeeded)
            return Ok();
        return BadRequest();
    }




    #region Ajax Request Handles

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        user.LastUpdatedOn = DateTime.Now;
        user.IsActive = !user.IsActive;
        user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        await _userManager.UpdateAsync(user);

        if (!user.IsActive)
            await _userManager.UpdateSecurityStampAsync(user);

        return Json(new { lastUpdatedOn = user.LastUpdatedOn.ToString() });
    }
    #endregion
}
