using Bookify.Web.Helper;
using Bookify.Web.Mapping;
using Bookify.Web.Services;
using Bookify.Web.Settings;
using Hangfire;
using HashidsNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using ViewToHTML.Extensions;
using WhatsAppCloudApi.Extensions;

namespace Bookify.Web.Extensions;

public static class DependancyInjection
{
    public static IServiceCollection AddBookifyServices(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddIdentity<ApplicationUser, IdentityRole>(option => option.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;
        });
        services.AddDataProtection().SetApplicationName(nameof(Bookify));

        services.AddControllersWithViews();

        services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<IEmailSender, EmailSender>();
        services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();

        services.AddAutoMapper(typeof(DomainProfile));
        services.AddExpressiveAnnotations();

        services.AddSingleton<IHashids>(_ => new Hashids(minHashLength: 11));

        services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

        //SecurityStampValidatorOptions: This class contains options for the security stamp validator.
        //The security stamp validator is responsible for checking the security stamp value of the user to
        //determine if it has changed and thus whether the user needs to be re-authenticated.
        //options.ValidationInterval : TimeSpan.Zero: The ValidationInterval property specifies how often the security stamp should be validated.
        //By setting this to TimeSpan.Zero, you�re instructing the validator to check the security stamp on every request.

        services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);
        services.AddWhatsAppApiClient(builder.Configuration);

        services.AddHangfire(x =>
        {
            x.UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString);
        });

        services.AddHangfireServer();

        services.Configure<AuthorizationOptions>(options =>
            options.AddPolicy("AdminsOnly", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole(AppRoles.Admin);
            }));

        // register ViewToHTML service
        services.AddViewToHTML();

        services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));


        return services;
    }
}
