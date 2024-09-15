namespace Bookify.Application.Common.Interfaces.Repositories;
public interface IBookRepository:IGenericRepository<Book>
{
    IQueryable<Book> GetDetails();
}
