using Bookify.Web.Core.Consts;

namespace Bookify.Web.Core.ViewModels;

public class BookCopyFormViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public bool IsAvailableForRental { get; set; }
    [Display(Name ="Edition Number"), Range(1, 1000, ErrorMessage = Errors.EditionNumberRange)]
    public int EditionNumber { get; set; }

    public bool IsBookAvailableForRental { get; set; }
}
