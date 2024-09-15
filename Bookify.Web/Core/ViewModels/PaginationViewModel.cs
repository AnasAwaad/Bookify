namespace Bookify.Web.Core.ViewModels;

public class PaginationViewModel
{
    public int PageNumber { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public int Start
    {
        get
        {
            var start = PageNumber;
            var numberOfPages = TotalPages - PageNumber + 1;
            var maxNumber = (int)ReportsConfiguration.MaxPaginationNumber;

            if (TotalPages < maxNumber)
            {
                start = 1;
            }
            else
            {
                if (numberOfPages < maxNumber)
                    start -= (maxNumber - numberOfPages);
            }

            return start;
        }
    }

    public int End
    {
        get
        {
            var maxNumber = (int)ReportsConfiguration.MaxPaginationNumber;
            var end = Start + maxNumber - 1;

            if (end > TotalPages)
                end = TotalPages;
            return end;
        }
    }
}
