namespace Bookify.Application.Common.Services.Categories;
public interface ICategoryService
{
    Category? GetById(int id);
    IEnumerable<Category> GetActiveCategories();
    IEnumerable<Category> GetAll();
    Category Add(string name,string createdById);
    Category? Update(int id, string name, string lastUpdatedById);
    bool IsCategoryAllowed(int id, string name);
    Category? ToggleStatus(int id, string lastUpdatedById);
}
