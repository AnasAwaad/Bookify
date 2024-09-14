using Bookify.Application.Common.Services.Authors;
using Bookify.Application.Common.Services.BookCopies;
using Bookify.Application.Common.Services.Books;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookCopyService, BookCopyService>();
        services.AddScoped<IBookService, BookService>();
        return services;
    }
}
