namespace Bookify.Domain.Common;

public class BaseEntity
{
    public BaseEntity()
    {
        CreatedOn = DateTime.Now;
        CreatedOn.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
        IsActive = true;
    }
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedOn { get; set; }

    public string? CreatedById { get; set; }
    public ApplicationUser? CreatedBy { get; set; }

    public string? LastUpdatedById { get; set; }
    public ApplicationUser? LastUpdatedBy { get; set; }
}
