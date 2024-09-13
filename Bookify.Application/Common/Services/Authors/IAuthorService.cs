namespace Bookify.Application.Common.Services.Authors;
public interface IAuthorService
{
    IEnumerable<Author> GetAll();
    Author? GetById(int id);
    Author Add(string name, string createdById);
    Author? Update(int authorId,string name,string lastUpdatedById);
    Author? ToggleStatus(int authorId,string lastUpdatedById);

    bool IsAuthorAllowed(int authorId,string name);

}
