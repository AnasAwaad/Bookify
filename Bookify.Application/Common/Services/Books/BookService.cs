using Bookify.Application.Common.Interfaces;

namespace Bookify.Application.Common.Services.Books;
internal class BookService:IBookService
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
}
