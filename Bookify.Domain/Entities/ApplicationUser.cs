

namespace Bookify.Domain.Entities;

[Index(nameof(UserName), IsUnique = true), Index(nameof(Email), IsUnique = true)]
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
