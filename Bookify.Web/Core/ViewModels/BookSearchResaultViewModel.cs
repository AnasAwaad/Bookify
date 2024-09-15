namespace Bookify.Web.Core.ViewModels;

public class BookSearchResaultViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public string ImageThumbnailUrl { get; set; }=null!;
    public string? Key { get; set; }
}
