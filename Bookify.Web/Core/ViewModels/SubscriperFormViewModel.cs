using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;
using UserManagement.Consts;

namespace Bookify.Web.Core.ViewModels;

public class SubscriperFormViewModel
{
	public string? Key { get; set; }
	[MaxLength(100,ErrorMessage =Errors.MaxLength)]
	public string FirstName { get; set; } = null!;
	[MaxLength(100, ErrorMessage = Errors.MaxLength)]
	public string LastName { get; set; } = null!;
	public DateTime DateOfBirth { get; set; }

	[MaxLength(20, ErrorMessage = Errors.MaxLength)]
	[Remote("IsAllowedNationalId","Subscripers",AdditionalFields ="Key", HttpMethod = "Post", ErrorMessage =Errors.Dublicated)]
	[RegularExpression(RegexPattern.NationalId,ErrorMessage =Errors.NotAllowedNationalId)]
	public string NationalId { get; set; } = null!;

	[MaxLength(11,ErrorMessage=Errors.MaxLength)]
	[RegularExpression(RegexPattern.PhoneNumber,ErrorMessage =Errors.Dublicated)]
	[Remote("IsAllowedMobileNumber", "Subscripers", AdditionalFields = "Key", HttpMethod = "Post", ErrorMessage = Errors.Dublicated)]
	public string MobileNumber { get; set; } = null!;
	public bool HasWhatsApp { get; set; }

	[MaxLength(150,ErrorMessage=Errors.MaxLength)]
	[EmailAddress]
	[Remote("IsAllowedEmail","Subscripers",AdditionalFields = "Key",HttpMethod ="Post",ErrorMessage =Errors.Dublicated)]
	public string Email { get; set; } = null!;

	[RequiredIf("Key == ''",ErrorMessage =Errors.RequiredField)]
	public IFormFile? Image { get; set; }
	public string? ImageUrl { get; set; }
	public string? ImageThumbnailUrl { get; set; }
	public int AreaId { get; set; }
	public Area? Area { get; set; }
	public int CityId { get; set; }
	public City? City { get; set; }

	[MaxLength(500, ErrorMessage = Errors.MaxLength)]
	public string Address { get; set; } = null!;
	public bool IsBlackListed { get; set; }

    public IEnumerable<SelectListItem> Cities { get; set; }=new List<SelectListItem>();
    public IEnumerable<SelectListItem>? Areas { get; set; }=new List<SelectListItem>();
}
