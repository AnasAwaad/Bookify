
using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.Auth;
internal class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;

	public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
	{
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public async Task<IEnumerable<ApplicationUser>> GetUsersAsync()
	{
		return await _userManager.Users.ToListAsync();
	}


	public async Task<ApplicationUser?> GetUsersByIdAsync(string id)
	{
		return await _userManager.FindByIdAsync(id);
	}

	public async Task<IList<string>> GetUsersRolesAsync(ApplicationUser user)
	{
		return await _userManager.GetRolesAsync(user);
	}

	public async Task<bool> IsLockedOutAsync(ApplicationUser user)
	{
		return await _userManager.IsLockedOutAsync(user);
	}
	public async Task<IEnumerable<IdentityRole>> GetRolesAsync()
	{
		return await _roleManager.Roles.ToListAsync();
	}

	public async Task<(ApplicationUser? user,bool isSucceeded, string? code,string? errors)> CreateAsync(CreateUserDto dto,string createdById)
	{

		var user = new ApplicationUser
		{
			FullName = dto.FullName,
			UserName = dto.UserName,
			Email = dto.Email,
			CreatedOn = DateTime.Now,
			CreatedById = createdById,
			IsActive = true
		};
		
		var result = await _userManager.CreateAsync(user, dto.Password);
		if (result.Succeeded)
		{
			await _userManager.AddToRolesAsync(user, dto.SelectedRoles);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			return (user,true,code,null);
		}

		var errors = string.Join(",", result.Errors.Select(e => e.Description));
		return (null,false, null,errors);
	}

	public async Task<(bool isSucceeded,ApplicationUser? user,string? errors)> UpdateAsync(ApplicationUser user, string lastUpdatedOn,IList<string> selectedRoles)
	{
		user.LastUpdatedById = lastUpdatedOn;
		user.LastUpdatedOn = DateTime.Now;

		var result = await _userManager.UpdateAsync(user);

		if (result.Succeeded)
		{
			var currentRoles = await _userManager.GetRolesAsync(user);
			var rolesUpdated = !currentRoles.SequenceEqual(selectedRoles);

			if (rolesUpdated)
			{
				await _userManager.RemoveFromRolesAsync(user, currentRoles);
				await _userManager.AddToRolesAsync(user, selectedRoles);
			}

			await _userManager.UpdateSecurityStampAsync(user);

			return (true,user,null);
		}

		var errors = string.Join(",", result.Errors.Select(e => e.Description));

		return (false,null, errors);
	}

	
	public async Task<(bool isSucceeded, ApplicationUser? user, string? errors)> ResetPasswordAsync(ApplicationUser user, string lastUpdatedById,string password)
	{
		var currentPasswordHash = user.PasswordHash;
		await _userManager.RemovePasswordAsync(user);

		var res = await _userManager.AddPasswordAsync(user, password);

		if (res.Succeeded)
		{
			user.LastUpdatedOn = DateTime.Now;
			user.LastUpdatedById = lastUpdatedById;

			await _userManager.UpdateAsync(user);

			return (true, user,null);
		}
		var errors = string.Join(",", res.Errors.Select(e => e.Description));

		return (false, null,errors);
	}

	public async Task<bool> IsAllowedUserName(string userName, string id)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
		return user is null || user.Id == id;
	}

	public async Task<bool> IsAllowedEmail(string email, string id)
	{
		var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
		return user is null || user.Id == id;
	}

	public async Task<bool> SetLockoutEndDateAsync(ApplicationUser user, DateTime date)
	{
		var res= await _userManager.SetLockoutEndDateAsync(user, date);
		return res.Succeeded;
	}

	public async Task<ApplicationUser?> ToggleStatus(string id,string lastUpdatedById)
	{
		var user = await GetUsersByIdAsync(id);

		if (user == null)
			return null;

		user.LastUpdatedOn = DateTime.Now;
		user.IsActive = !user.IsActive;
		user.LastUpdatedById = lastUpdatedById;

		await _userManager.UpdateAsync(user);

		if (!user.IsActive)
			await _userManager.UpdateSecurityStampAsync(user);
		return user;
	}
}
