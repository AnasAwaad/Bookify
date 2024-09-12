namespace Bookify.Web.Core.Models;

public class Subscription
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int SubscriperId { get; set; }
    public Subscriper? Subscriper { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }

}
