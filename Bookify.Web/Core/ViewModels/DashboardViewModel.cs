namespace Bookify.Web.Core.ViewModels;

public class DashboardViewModel
{
    public int NumberOfBooks { get; set; }
    public int NumberOfSubscriber { get; set; }

    public IEnumerable<BookViewModel> LatestBooks { get; set; }=new List<BookViewModel>();
    public IEnumerable<BookViewModel> TopBooks { get; set; }=new List<BookViewModel>();
}
