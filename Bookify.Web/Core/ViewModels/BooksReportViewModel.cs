using Bookify.Web.Core.Utilities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.ViewModels;

public class BooksReportViewModel
{

    public IEnumerable<SelectListItem> Authors { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

    [Display(Name = "Authors")]
    public List<int>? SelectedAuthors { get; set; } = new List<int>();

    [Display(Name = "Categories")]
    public List<int>? SelectedCategories { get; set; } = new List<int>();

    public PaginatedList<Book> Data { get; set; }
}
