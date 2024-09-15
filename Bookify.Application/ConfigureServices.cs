using Bookify.Application.Common.Services.Authors;
using Bookify.Application.Common.Services.BookCopies;
using Bookify.Application.Common.Services.Books;
using Bookify.Application.Common.Services.Categories;
using Bookify.Application.Common.Services.RentalCopies;
using Bookify.Application.Common.Services.RentalService;
using Bookify.Application.Common.Services.Subscripers;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookCopyService, BookCopyService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubscriperService, SubscriperService>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<IRentalCopiesService, RentalCopiesService>();
        return services;
    }
}
