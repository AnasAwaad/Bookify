﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookify.Web.Core.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
    [ValidateNever]
    public bool IsLockedOut { get; set; }

}
