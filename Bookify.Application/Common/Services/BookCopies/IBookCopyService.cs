using Bookify.Application.Common.Interfaces.Repositories;

namespace Bookify.Application.Common.Services.BookCopies;
public interface IBookCopyService
{
    BookCopy? GetById(int id);
    BookCopy? GetDetails(int id);
    BookCopy? ToggleStatus(int id, string lastUpdatedById);

    BookCopy? Add(int bookId,bool isAvailableForRental, int editionNumber, string CreatedById);

    BookCopy? Update(int id,bool isAvailableForRental, int editionNumber, string lastUpdatedById);
}
