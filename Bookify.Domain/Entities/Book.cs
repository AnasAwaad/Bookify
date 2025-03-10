﻿namespace Bookify.Domain.Entities;

public class Book : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int AuthorId { get; set; }
    public Author? Author { get; set; }
    public string Publisher { get; set; } = null!;
    public DateTime PublishingDate { get; set; }
    public string? ImageUrl { get; set; }
    public string Hall { get; set; } = null!;
    public string? ImageThumbnailUrl { get; set; }
    public int? ImagePublicId { get; set; }
    public bool IsAvailableForRental { get; set; }
    public string Description { get; set; } = null!;
    public ICollection<BookCategory> Categories { get; set; } = new List<BookCategory>();
    public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>();
}
