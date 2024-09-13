using Bookify.Application.Common.Services.Authors;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthorService, AuthorService>();
        return services;
    }
}
