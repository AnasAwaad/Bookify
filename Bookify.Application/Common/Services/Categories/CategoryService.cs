
using Bookify.Application.Common.Interfaces;

namespace Bookify.Application.Common.Services.Categories;
internal class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public Category? GetById(int id)
    {
        return _unitOfWork.Categories.GetById(id);
    }
    public IEnumerable<Category> GetActiveCategories()
    {
        return _unitOfWork.Categories.FindAll(c=>c.IsActive ,c=>c.Name);
    }

    public IEnumerable<Category> GetAll()
    {
        return _unitOfWork.Categories.GetAll();
    }

    public Category Add(string name, string createdById)
    {
        var category = new Category
        {
            Name = name,
            CreatedById = createdById,
            CreatedOn = DateTime.Now,
        };

        _unitOfWork.Categories.Add(category);
        _unitOfWork.SaveChanges();

        return category;
    }

    public Category? Update(int id, string name,string lastUpdatedById)
    {
        var category=_unitOfWork.Categories.GetById(id);
        if (category is null)
            return null;

        category.Name = name;
        category.LastUpdatedOn = DateTime.Now;
        category.LastUpdatedById = lastUpdatedById;

        _unitOfWork.SaveChanges();

        return category;
    }

    public bool IsCategoryAllowed(int id, string name)
    {
        var category = _unitOfWork.Categories.Find(c => c.Name == name);

        return category is null || category.Id == id;
    }

    public Category? ToggleStatus(int id, string lastUpdatedById)
    {
        var category = _unitOfWork.Categories.GetById(id);
        if (category == null)
            return null;

        category.LastUpdatedOn = DateTime.Now;
        category.IsActive = !category.IsActive;
        category.LastUpdatedById = lastUpdatedById; ;

        _unitOfWork.SaveChanges();

        return category;
    }
}
