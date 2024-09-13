namespace Bookify.Domain.Entities;

public class BookCopy : BaseEntity
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public bool IsAvailableForRental { get; set; }
    public int EditionNumber { get; set; }//رقم الطابعة
    public int SerialNumber { get; set; }

    public ICollection<RentalCopy> RentalCopies { get; set; } = new List<RentalCopy>();
}
