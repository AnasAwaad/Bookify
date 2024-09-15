using Bookify.Domain.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Bookify.Application.Common.Services.Auth;
public interface IAuthService
{
	Task<IEnumerable<ApplicationUser>> GetUsersAsync();
	Task<ApplicationUser?> GetUsersByIdAsync(string id);
	Task<IList<string>> GetUsersRolesAsync(ApplicationUser user);
	Task<bool> IsLockedOutAsync(ApplicationUser user);
	Task<bool> SetLockoutEndDateAsync(ApplicationUser user,DateTime date);
	Task<IEnumerable<IdentityRole>> GetRolesAsync();
	Task<(ApplicationUser? user, bool isSucceeded, string? code, string? errors)> CreateAsync(CreateUserDto dto, string createdById);
	Task<(bool isSucceeded, ApplicationUser? user, string? errors)> UpdateAsync(ApplicationUser user, string lastUpdatedOn, IList<string>? selectedRoles);
	Task<(bool isSucceeded, ApplicationUser? user, string? errors)> ResetPasswordAsync(ApplicationUser user, string lastUpdatedById, string password);
	Task<bool> IsAllowedUserName(string userName,string id);
	Task<bool> IsAllowedEmail(string email,string id);
	Task<ApplicationUser?> ToggleStatus(string id,string lastUpdatedById);
}
