﻿namespace Bookify.Web.Core.ViewModels;

public class BookViewModel
{
    public int Id { get; set; }
    public string? Key { get; set; }
    public string Title { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public string Publisher { get; set; } = null!;
    public DateTime PublishingDate { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? ImageThumbnailUrl { get; set; } = null!;
    public string Hall { get; set; } = null!;
    public bool IsAvailableForRental { get; set; }
    public string Description { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public ICollection<string> Categories { get; set; } = null!;
    public ICollection<BookCopyViewModel> BookCopies { get; set; } = null!;
}
