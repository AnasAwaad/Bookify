using Bookify.Application.Common.Models;
using Bookify.Domain.Dtos;

namespace Bookify.Application.Common.Services.Books;
public interface IBookService
{
    Book? GetById(int id);
    Book? GetWithCategories(int id);
    IQueryable<Book> GetDetails();
    (IQueryable<Book> books, int count) GetFiltered(FilteredDto dto);
    int GetActiveBooksCount();
    IEnumerable<LatestBookDto> GetLatestBooks(int count);
    IEnumerable<TopBookDto> GetTopBooks(int count);
    PaginatedList<Book> GetPaginatedList(int pageNumber, List<int> selectedCategories, List<int> selectedAuthors);
    IQueryable<Book> GetQurableRowData(string authors, string categories);
    Book Add(Book book,IList<int> selectedCategories,string createdById);
    Book Update(Book book,IList<int> selectedCategories,string LastUpdatedById);
    bool IsBookAllowed(int id,string bookTitle,int authorId);
    Book? ToggleStatus(int id, string lastUpdatedById);
    IQueryable<Book>? Search(string query);
}
