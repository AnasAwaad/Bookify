using Bookify.Application.Common.Interfaces.Repositories;

namespace Bookify.Application.Common.Interfaces;
public interface IUnitOfWork
{
    public IGenericRepository<Author> Authors { get; }
    public IBookRepository Books { get; }
    public IGenericRepository<BookCopy> BookCopies { get; }
    public IGenericRepository<Category> Categories { get; }
    int SaveChanges();
}
