namespace Bookify.Domain.Entities;

public class City : BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<Area> Areas { get; set; } = new List<Area>();
}
