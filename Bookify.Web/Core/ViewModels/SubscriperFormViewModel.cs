using Microsoft.AspNetCore.Mvc.Rendering;
using UserManagement.Consts;

namespace Bookify.Web.Core.ViewModels;

public class SubscriperFormViewModel
{
	public int Id { get; set; }
	[MaxLength(100,ErrorMessage =Errors.MaxLength)]
	public string FirstName { get; set; } = null!;
	[MaxLength(100, ErrorMessage = Errors.MaxLength)]
	public string LastName { get; set; } = null!;
	public DateTime DateOfBirth { get; set; }

	[MaxLength(20, ErrorMessage = Errors.MaxLength)]
	[Remote("IsAllowedNationalId","Subscripers","Id", HttpMethod = "Post", ErrorMessage =Errors.Dublicated)]
	[RegularExpression(RegexPattern.NationalId,ErrorMessage =Errors.NotAllowedNationalId)]
	public string NationalId { get; set; } = null!;

	[MaxLength(11,ErrorMessage=Errors.MaxLength)]
	[RegularExpression(RegexPattern.PhoneNumber)]
	[Remote("IsAllowedMobileNumber", "Subscripers", "Id", HttpMethod = "Post", ErrorMessage = Errors.Dublicated)]
	public string MobileNumber { get; set; } = null!;
	public bool HasWhatsApp { get; set; }

	[MaxLength(150,ErrorMessage=Errors.MaxLength)]
	[EmailAddress]
	[Remote("IsAllowedEmail","Subscripers","Id",HttpMethod ="Post",ErrorMessage =Errors.Dublicated)]
	public string Email { get; set; } = null!;
	public IFormFile Image { get; set; } = null!;
	public int AreaId { get; set; }
	public Area? Area { get; set; }
	public int CityId { get; set; }
	public City? City { get; set; }

	[MaxLength(500, ErrorMessage = Errors.MaxLength)]
	public string Address { get; set; } = null!;
	public bool IsBlackListed { get; set; }

    public IEnumerable<SelectListItem> Cities { get; set; }=new List<SelectListItem>();
}
