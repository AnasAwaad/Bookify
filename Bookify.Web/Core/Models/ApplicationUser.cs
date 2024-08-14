﻿using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Core.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string FullName { get; set; } = null!;

	public bool IsActive { get; set; }
	public DateTime CreatedOn { get; set; }
	public DateTime? LastUpdatedOn { get; set; }
    public string? CreatedById { get; set; }
    public string? LastUpdatedById { get; set; }    
}
