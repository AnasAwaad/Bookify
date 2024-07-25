namespace Bookify.Web.Core.Models;

[Index("Title","AuthorId",IsUnique =true)]
public class Book : BaseModel
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Title { get; set; } = null!;
    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public string Description { get; set; }=null!;

    [MaxLength(50)]
    public string Publisher { get; set; } = null!;
    public DateTime PublishingDate { get; set; }

    public string? ImageUrl { get; set; }

    public ICollection<BookCategory>? Categories { get; set; } = new List<BookCategory>();
}
