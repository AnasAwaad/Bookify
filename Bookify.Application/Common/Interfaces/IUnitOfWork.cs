using Bookify.Application.Common.Interfaces.Repositories;

namespace Bookify.Application.Common.Interfaces;
public interface IUnitOfWork
{
    public IGenericRepository<Author> Authors { get; }
    public IGenericRepository<Area> Areas { get; }
    public IBookRepository Books { get; }
    public IGenericRepository<BookCopy> BookCopies { get; }
    public IGenericRepository<Category> Categories { get; }
    public IGenericRepository<City> Cities { get; }
    public IGenericRepository<Rental> Rentals { get; }
    public IGenericRepository<RentalCopy> RentalCopies { get; }
    public IGenericRepository<Subscriper> Subscripers { get; }
    int SaveChanges();
}
