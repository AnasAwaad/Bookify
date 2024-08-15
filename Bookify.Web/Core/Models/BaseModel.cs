using System.Globalization;

namespace Bookify.Web.Core.Models;

public class BaseModel
{
    public BaseModel()
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
