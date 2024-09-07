namespace Bookify.Web.Core.ViewModels;

public class RentalViewModel
{
	public int Id { get; set; }
	public SubscriberViewModel? Subscriper { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime CreatedOn { get; set; }
	public bool PenaltyPaid { get; set; }
	public ICollection<RentalCopyViewModel> RentalCopies { get; set; } = new List<RentalCopyViewModel>();
    public int TotalDelayInDays { 
		get
		{
			return RentalCopies.Sum(rc=>rc.DelayInDays);
		}
	}

    public int TotalCopies {
		get
		{
			return RentalCopies.Count();
		}
	}
}
