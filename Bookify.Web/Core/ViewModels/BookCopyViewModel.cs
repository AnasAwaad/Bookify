namespace Bookify.Web.Core.ViewModels;

public class BookCopyViewModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Title { get; set; } = null!;
    public string ImageThumbnailUrl { get; set; } = null!;
    public bool IsAvailableForRental { get; set; }
    public int EditionNumber { get; set; }//رقم الطابعة
    public int SerialNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}
