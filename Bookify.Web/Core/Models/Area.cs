namespace Bookify.Web.Core.Models;


[Index(nameof(Name), nameof(CityId), IsUnique = true)]
public class Area : BaseModel
{
    public int Id { get; set; }
    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int CityId { get; set; }
    public City City { get; set; } = null!;
}
