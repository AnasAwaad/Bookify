namespace Bookify.Web.Core.Models;

[Index(nameof(NationalId),IsUnique =true)]
[Index(nameof(MobileNumber),IsUnique =true)]
[Index(nameof(Email),IsUnique =true)]
public class Subscriper : BaseModel
{
    public int Id { get; set; }
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    [StringLength(20)]
    public string NationalId { get; set; } = null!;

    [StringLength(15)]
    public string MobileNumber { get; set; }=null!;
    public bool HasWhatsApp { get; set; }

    [StringLength(150)]
    public string Email { get; set; } = null!;

    [StringLength(500)]
    public string ImageUrl { get; set; } = null!;
    [StringLength(500)]
    public string ImageThumbnailUrl { get; set; } = null!;
    public int AreaId { get; set; }
    public Area? Area { get; set; }
    public int CityId { get; set; }
    public City? City { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
    public bool IsBlackListed{ get; set; }



}
