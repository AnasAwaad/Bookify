using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.ViewModels;

public class BookFormViewModel
{
	public int Id { get; set; }

	[MaxLength(100,ErrorMessage =Errors.MaxLength)]
	public string Title { get; set; } = null!;

	[Display(Name ="Author")]
	public int AuthorId { get; set; }

	[MaxLength(50, ErrorMessage = Errors.MaxLength)]
	public string Publisher { get; set; } = null!;

	[Display(Name = "Publishing Date")]
	public DateTime PublishingDate { get; set; }

	public IFormFile? ImageFile { get; set; }

	[MaxLength(50, ErrorMessage = Errors.MaxLength)]
	public string Hall { get; set; } = null!;

	[Display(Name = "Is avaliable for rental")]
	public bool IsAvaliableForRental { get; set; }

	public string Description { get; set; } = null!;
	// why
	public IList<int> SelectedCategories { get; set; } = new List<int>();
	public IEnumerable<SelectListItem>? AuthorSelectList { get; set; } = new List<SelectListItem>();
	public IEnumerable<SelectListItem>? CategorySelectList { get; set; }=new List<SelectListItem>();	


}
