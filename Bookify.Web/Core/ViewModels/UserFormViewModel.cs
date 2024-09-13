using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;
using UserManagement.Consts;

namespace Bookify.Web.Core.ViewModels;

public class UserFormViewModel
{
    public string? Id { get; set; }

    [Display(Name = "Full Name"), MaxLength(100, ErrorMessage = Errors.MaxLength)]
    [RegularExpression(RegexPattern.CharactersOnly_Eng, ErrorMessage = Errors.OnlyEnglishLetters)]
    public string FullName { get; set; } = null!;

    [Display(Name = "Username"), MaxLength(20, ErrorMessage = Errors.MaxLength)]
    [RegularExpression(RegexPattern.Username, ErrorMessage = Errors.AllowUsername)]
    [Remote("AllowUserName", null!,  AdditionalFields = "Id", ErrorMessage = Errors.Dublicated)]
    public string UserName { get; set; } = null!;

    [EmailAddress, MaxLength(200, ErrorMessage = Errors.MaxLength)]
    [Remote("AllowEmail", null!, AdditionalFields = "Id", ErrorMessage = Errors.Dublicated)]
    public string Email { get; set; } = null!;

    [StringLength(100, MinimumLength = 6, ErrorMessage = Errors.MinMaxLength), DataType(DataType.Password)]
    [RequiredIf("Id==null", ErrorMessage = Errors.RequiredField)]
    [RegularExpression(RegexPattern.Password, ErrorMessage = Errors.WeakPassword)]
    public string? Password { get; set; } = null!;

    [DataType(DataType.Password), Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = Errors.ConfirmPasswordNotMatch)]
    [RequiredIf("Id==null", ErrorMessage = Errors.RequiredField)]
    public string? ConfirmPassword { get; set; } = null!;

    public IList<string> SelectedRoles { get; set; } = new List<string>();
    public IEnumerable<SelectListItem>? RolesSelectList { get; set; } = new List<SelectListItem>();
}
