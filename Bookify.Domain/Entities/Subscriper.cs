namespace Bookify.Domain.Entities;

public class Subscriper : BaseEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string NationalId { get; set; } = null!;
    public string MobileNumber { get; set; } = null!;
    public bool HasWhatsApp { get; set; }
    public string Email { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string ImageThumbnailUrl { get; set; } = null!;
    public int AreaId { get; set; }
    public Area? Area { get; set; }
    public int CityId { get; set; }
    public City? City { get; set; }
    public string? Address { get; set; }
    public bool IsBlackListed { get; set; }
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
