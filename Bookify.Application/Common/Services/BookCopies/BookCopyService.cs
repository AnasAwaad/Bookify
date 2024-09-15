using Bookify.Application.Common.Interfaces;
using Bookify.Application.Common.Interfaces.Repositories;
using Bookify.Domain.Consts;
using Bookify.Domain.Entities;
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

    public (string? errorMessage,ICollection<RentalCopy>? copies) CanBeRentaled(IList<int> selectedCopiesItems,int subscriperId)
    {
        
        var selectedCopies = _unitOfWork.BookCopies
            .FindAll(
                c => selectedCopiesItems.Contains(c.SerialNumber),
                c=>c.Include(x=>x.Book).Include(x=>x.RentalCopies))
            .ToList();

        var currentSubscriperRentals = _unitOfWork.Rentals.GetQueryable()
            .Include(r => r.RentalCopies)
            .ThenInclude(c => c.BookCopy)
            .Where(r => r.SubscriperId == subscriperId)
            .SelectMany(r => r.RentalCopies)
            .Where(c => !c.ReturnDate.HasValue)
            .Select(r => r.BookCopy!.BookId);

        List<RentalCopy> copies = new();
        foreach (var copy in selectedCopies)
        {
            if (!copy.IsAvailableForRental || !copy.Book!.IsAvailableForRental)
                return (Errors.NotAvailableForRental,null);

            if (copy.RentalCopies.Any(r => !r.ReturnDate.HasValue))
                return (Errors.CopyInRental, null);

            if (currentSubscriperRentals.Any(bookId => bookId == copy.BookId))
                return ($"This subscriber already has a copy for '{copy.Book.Title}' Book",null);

            copies.Add(new RentalCopy { BookCopyId = copy.Id });
        }

        return (null, copies);

    }

    public BookCopy? SearchForBookCopy(string value)
    {
        var copy = _unitOfWork.BookCopies.GetQueryable()
            .Include(c => c.Book)
            .SingleOrDefault(c => c.SerialNumber.ToString().Equals(value) && c.IsActive && c.Book!.IsActive);

        return copy is null ? null : copy;
    }
}
