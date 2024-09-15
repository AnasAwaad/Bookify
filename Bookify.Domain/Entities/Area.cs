using Bookify.Domain.Common;
namespace Bookify.Domain.Entities;

public class Area : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int CityId { get; set; }
    public City City { get; set; } = null!;
}
