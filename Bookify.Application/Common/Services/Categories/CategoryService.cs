
using Bookify.Application.Common.Interfaces;

namespace Bookify.Application.Common.Services.Categories;
internal class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IEnumerable<Category> GetActiveCategories()
    {
        return _unitOfWork.Categories.FindAll(c=>c.IsActive ,c=>c.Name);
    }
}
