using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModels;

public class BookFormViewModel
{
	public int Id { get; set; }

	[MaxLength(100,ErrorMessage =Errors.MaxLength)]
	[Remote("IsBookAllowed","Books",AdditionalFields ="Id,AuthorId", HttpMethod = "Post",ErrorMessage =Errors.BookAuthorDublicated)]
	public string Title { get; set; } = null!;

	[Display(Name ="Author")]
	[Remote("IsBookAllowed", "Books", AdditionalFields = "Id,Title", HttpMethod = "Post", ErrorMessage = Errors.BookTitleDublicated)]
	public int AuthorId { get; set; }

	[MaxLength(50, ErrorMessage = Errors.MaxLength)]
	public string Publisher { get; set; } = null!;

	[Display(Name = "Publishing Date")]
	[AssertThat("PublishingDate <= Today()",ErrorMessage =Errors.NotAllowedDate)]
	public DateTime PublishingDate { get; set; }= DateTime.Now;

	public IFormFile? ImageFile { get; set; }
	public string? ImageUrl { get; set; }
	public string? ImageThumbnailUrl { get; set; } = null!;
	public int? ImagePublicId { get; set; }

	[MaxLength(50, ErrorMessage = Errors.MaxLength)]
	public string Hall { get; set; } = null!;

	[Display(Name = "Is avaliable for rental")]
	public bool IsAvailableForRental { get; set; }

	public string Description { get; set; } = null!;
	// why
	public IList<int> SelectedCategories { get; set; } = new List<int>();
	public IEnumerable<SelectListItem>? AuthorSelectList { get; set; } = new List<SelectListItem>();
	public IEnumerable<SelectListItem>? CategorySelectList { get; set; }=new List<SelectListItem>();	


}
