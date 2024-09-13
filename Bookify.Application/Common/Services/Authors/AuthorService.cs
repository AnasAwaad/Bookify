
using Bookify.Application.Common.Interfaces;
using System.Xml.Linq;

namespace Bookify.Application.Common.Services.Authors;
internal class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _unitOfWork;


    public AuthorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Author Add(string name,string createdById)
    {
        var author=new Author {
            Name = name,
            CreatedById = createdById,
            CreatedOn=DateTime.Now
        };

        _unitOfWork.Authors.Add(author);
        _unitOfWork.SaveChanges();

        return author;
    }
    public Author? GetById(int id)
    {
        return _unitOfWork.Authors.GetById(id); 
    }

    public IEnumerable<Author> GetAll()
    {
        return _unitOfWork.Authors.GetAll();
    }

    public Author? Update(int authorId, string name, string lastUpdatedById)
    {
        var author = GetById(authorId);
        if (author == null)
            return null;

        author.Name = name;
        author.LastUpdatedById = lastUpdatedById;
        author.LastUpdatedOn = DateTime.Now;

        _unitOfWork.Authors.Update(author);
        _unitOfWork.SaveChanges();
        return author;
    }

    public Author? ToggleStatus(int authorId, string lastUpdatedById)
    {
        var author = GetById(authorId);
        if (author == null)
            return null;

        author.IsActive = !author.IsActive;
        author.LastUpdatedById = lastUpdatedById;
        author.LastUpdatedOn = DateTime.Now;

        _unitOfWork.Authors.Update(author);
        _unitOfWork.SaveChanges();
        return author;
    }

    public bool IsAuthorAllowed(int authorId, string name)
    {
        var author = _unitOfWork.Authors.Find(a => a.Name == name);
        return author is null || author.Id == authorId;
    }
}
