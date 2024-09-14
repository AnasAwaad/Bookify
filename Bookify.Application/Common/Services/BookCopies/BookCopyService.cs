using Bookify.Application.Common.Interfaces;
using Bookify.Application.Common.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Application.Common.Services.BookCopies;
internal class BookCopyService : IBookCopyService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookCopyService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public BookCopy? Add(int bookId, bool isAvailableForRental, int editionNumber, string CreatedById)
    {
        var book = _unitOfWork.Books.GetById(bookId);
        if (book == null)
            return null;

        var bookCopy = new BookCopy()
        {
            IsAvailableForRental = isAvailableForRental,
            EditionNumber = editionNumber,
            CreatedById =   CreatedById,
            CreatedOn = DateTime.Now
        };

        book.BookCopies.Add(bookCopy);

        _unitOfWork.SaveChanges();

        return bookCopy;
    }

    public BookCopy? GetById(int id)
    {
        return _unitOfWork.BookCopies.GetById(id);
    }

    public BookCopy? GetDetails(int id)
    {
        return _unitOfWork.BookCopies.Find(b => b.Id == id, include : b => b.Include(x => x.Book!));
    }

    public BookCopy? ToggleStatus(int id,string lastUpdatedById)
    {
        var bookCopy=GetById(id);
        if (bookCopy is null)
            return null;
        bookCopy.IsActive = !bookCopy.IsActive;
        bookCopy.LastUpdatedOn = DateTime.Now;
        bookCopy.LastUpdatedById = lastUpdatedById;

        _unitOfWork.SaveChanges(); 
        return bookCopy;
    }

    public BookCopy? Update(int id, bool isAvailableForRental, int editionNumber, string lastUpdatedById)
    {
        var bookCopy = GetById(id);
        if (bookCopy is null) 
            return null;

        bookCopy.EditionNumber = editionNumber;
        bookCopy.LastUpdatedOn = DateTime.Now;
        bookCopy.IsAvailableForRental = isAvailableForRental;
        bookCopy.LastUpdatedById = lastUpdatedById;

        _unitOfWork.SaveChanges();

        return bookCopy;
    }
}
