namespace Bookify.Web.Core.Models;

public class BookCopy : BaseModel
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public bool IsAvailableForRental { get; set; }
    public int EditionNumber { get; set; }//رقم الطابعة
    public int SerialNumber { get; set; }

}
