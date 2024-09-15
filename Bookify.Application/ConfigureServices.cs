using Bookify.Application.Common.Services.Areas;
using Bookify.Application.Common.Services.Auth;
using Bookify.Application.Common.Services.Authors;
using Bookify.Application.Common.Services.BookCopies;
using Bookify.Application.Common.Services.Books;
using Bookify.Application.Common.Services.Categories;
using Bookify.Application.Common.Services.Cities;
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
		services.AddScoped<IAreaService, AreaService>();
		services.AddScoped<IAuthService, AuthService>();
		services.AddScoped<IBookCopyService, BookCopyService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ISubscriperService, SubscriperService>();
        services.AddScoped<IRentalService, RentalService>();
        services.AddScoped<IRentalCopiesService, RentalCopiesService>();
        return services;
    }
}
