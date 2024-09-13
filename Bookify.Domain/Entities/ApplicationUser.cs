

namespace Bookify.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }
    public string? CreatedById { get; set; }
    public string? LastUpdatedById { get; set; }
}
