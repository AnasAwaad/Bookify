using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Bookify.Web.Controllers;
[Authorize(Roles =AppRoles.Admin)]
public class UsersController : Controller
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IMapper _mapper;

	public UsersController(UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_mapper = mapper;
		_roleManager = roleManager;
	}

	public async Task<IActionResult> Index()
	{
		var users=await _userManager.Users.ToListAsync();
		var viewModel=_mapper.Map<IEnumerable<UserViewModel>>(users);
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
		return PartialView("_Form",viewModel);
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
			CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
			
		};

		var result=await _userManager.CreateAsync(user, model.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRolesAsync(user, model.SelectedRoles);
            return PartialView("_UserRow", _mapper.Map<UserViewModel>(user));
        }
        return BadRequest();
	}
}
