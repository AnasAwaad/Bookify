using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.ViewModels;

public class UserFormViewModel
{
    public string? Id { get; set; }

    [Display(Name ="Full Name"),MaxLength(100,ErrorMessage=Errors.MaxLength)]
    public string FullName { get; set; } = null!;

	[Display(Name = "Username"),MaxLength(20,ErrorMessage =Errors.MaxLength)]
	public string UserName { get; set; } = null!;

	[EmailAddress,MaxLength(200,ErrorMessage =Errors.MaxLength)]
	public string Email { get; set; }=null!;

	[StringLength(100, MinimumLength = 6, ErrorMessage = Errors.MinMaxLength), DataType(DataType.Password)]
	public string Password { get; set; } = null!;

	[DataType(DataType.Password), Display(Name = "Confirm password")]
	[Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
	public string ConfirmPassword { get; set; } = null!;

	public IList<string> SelectedRoles { get; set; } = new List<string>();
	public IEnumerable<SelectListItem>? RolesSelectList { get; set; } = new List<SelectListItem>();
}
