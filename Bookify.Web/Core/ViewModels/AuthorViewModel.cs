﻿namespace Bookify.Web.Core.ViewModels;

public class AuthorViewModel
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
}
