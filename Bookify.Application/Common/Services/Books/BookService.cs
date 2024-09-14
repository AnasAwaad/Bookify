using Bookify.Application.Common.Interfaces;
using Bookify.Domain.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Buffers;
using System.Linq.Dynamic.Core;

namespace Bookify.Application.Common.Services.Books;
internal class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Book? GetById(int id)
    {
        return _unitOfWork.Books.GetById(id);
    }

    public Book? GetWithCategories(int id)
    {
        return _unitOfWork.Books.Find(b => b.Id == id, include: b => b.Include(x => x.Categories));
    }

    public IQueryable<Book> GetDetails()
    {
        return _unitOfWork.Books.GetDetails();
    }



    public (IQueryable<Book> books, int count) GetFiltered(FilteredDto dto)
    {
        var countOfBooks = _unitOfWork.Books.Count();
        var query = _unitOfWork.Books.GetDetails();

        if (!string.IsNullOrEmpty(dto.SearchValue))
            query = query.Where(b => b.Title.Contains(dto.SearchValue!) || b.Author!.Name.Contains(dto.SearchValue!));

        query = query.OrderBy($"{dto.OrderColumnName} {dto.OrderColumnDirection}");  //orderBy from system.Linq.Dynamic lib

        var data = query.Skip(dto.Skip).Take(dto.PageSize);

        return (data, countOfBooks);
    }

    public int GetActiveBooksCount()
    {
        return _unitOfWork.Books.Count(b => b.IsActive);
    }

    public IEnumerable<LatestBookDto> GetLatestBooks(int count)
    {
        var books = _unitOfWork.Books.GetQueryable();
        var latestBooks = books
            .Include(b => b.Author)
            .Where(b => b.IsActive)
            .OrderByDescending(b => b.PublishingDate)
            .Take(count)
            .Select(b => new LatestBookDto(
                b.Id,
                b.ImageThumbnailUrl!,
                b.Title,
                b.Author!.Name
            ))
            .ToList();

        return latestBooks;
    }

    public IEnumerable<TopBookDto> GetTopBooks(int count)
    {

        var topBooks = _unitOfWork.RentalCopies.GetQueryable();

        return topBooks.Include(r => r.BookCopy)
            .ThenInclude(c => c!.Book)
            .ThenInclude(b => b!.Author)
            .GroupBy(r => new
            {
                r.BookCopy!.BookId,
                r.BookCopy!.Book!.Title,
                r.BookCopy!.Book!.ImageThumbnailUrl,
                AuthorName = r.BookCopy!.Book!.Author!.Name,

            }).Select(b => new
            {
                b.Key.BookId,
                b.Key.AuthorName,
                b.Key.Title,
                b.Key.ImageThumbnailUrl,
                Count = b.Count()
            })
            .OrderByDescending(b => b.Count)
            .Take(count)
            .Select(b => new TopBookDto(b.BookId, b.AuthorName, b.Title, b.ImageThumbnailUrl!))
            .ToList();

            
    }

    public Book Add(Book book, IList<int> selectedCategories, string createdById)
    {
        // add categories for new book 
        foreach (var category in selectedCategories)
            book.Categories.Add(new BookCategory() { CategoryId = category });

        book.CreatedOn = DateTime.Now;
        book.CreatedById = createdById;

        _unitOfWork.Books.Add(book);
        _unitOfWork.SaveChanges();

        return book;
    }

    public Book Update(Book book, IList<int> selectedCategories, string LastUpdatedById)
    {

        book.LastUpdatedOn = DateTime.Now;
        book.LastUpdatedById = LastUpdatedById;
        book.Categories = new List<BookCategory>();

        foreach (var category in selectedCategories)
            book.Categories.Add(new BookCategory { CategoryId = category });

        _unitOfWork.SaveChanges();
        return book;
    }

    public bool IsBookAllowed(int id, string bookTitle, int authorId)
    {
        var book = _unitOfWork.Books.Find(c => c.Title == bookTitle && c.AuthorId == authorId);

        return book == null || book.Id == id;
    }

    public Book? ToggleStatus(int id, string lastUpdatedById)
    {
        var book = GetById(id);
        if (book is null)
            return null;
        book.IsActive = !book.IsActive;
        book.LastUpdatedOn = DateTime.Now;
        book.LastUpdatedById = lastUpdatedById;

        _unitOfWork.SaveChanges();

        return book;
    }

    public IQueryable<Book>? Search(string query)
    {
        var books = _unitOfWork.Books.GetQueryable();

        return books
            .Include(b => b.Author)
            .Where(b => b.IsActive && (b.Title.Contains(query) || b.Author!.Name.Contains(query)));
    }
}
