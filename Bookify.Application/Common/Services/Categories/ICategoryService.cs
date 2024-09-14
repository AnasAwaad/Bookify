namespace Bookify.Application.Common.Services.Categories;
public interface ICategoryService
{
    IEnumerable<Category> GetActiveCategories();
}
