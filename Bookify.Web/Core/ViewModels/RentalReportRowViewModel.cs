namespace Bookify.Web.Core.ViewModels;

public class RentalReportRowViewModel
{
    public int SubscriperId { get; set; }
    public string SubscriberName { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public string BookTitle { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime RentalDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime? ExtendedOn { get; set; }

}
