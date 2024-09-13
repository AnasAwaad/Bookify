
using Bookify.Application.Common.Interfaces;

namespace Bookify.Infrastructure.Persistence.Repositories;
public class BookRepository : GenericRepository<Book>,IBookRepository
{
    public BookRepository(ApplicationDbContext context) : base(context)
    {
    }

    public IQueryable<Book> GetDetails()
    {
        return _context.Books
             .Include(b => b.BookCopies)
             .Include(b => b.Author)
             .Include(b => b.Categories)
             .ThenInclude(b => b.Category);
    }
}
