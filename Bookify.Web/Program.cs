using Bookify.Web.Helper;
using Bookify.Web.Mapping;
using Bookify.Web.Seeds;
using Bookify.Web.Services;
using Bookify.Web.Settings;
using Humanizer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Channels;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;

namespace Bookify.Web
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option=>option.SignIn.RequireConfirmedAccount=true)
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultUI()
				.AddDefaultTokenProviders();

			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Default Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 6;
				options.Password.RequiredUniqueChars = 1;
			});

			builder.Services.AddControllersWithViews();

			builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
			builder.Services.AddTransient<IImageService, ImageService>();
			builder.Services.AddTransient<IEmailSender, EmailSender>();
			builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();

			builder.Services.AddAutoMapper(typeof(DomainProfile));
			builder.Services.AddExpressiveAnnotations();
			builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

			//SecurityStampValidatorOptions: This class contains options for the security stamp validator.
			//The security stamp validator is responsible for checking the security stamp value of the user to
			//determine if it has changed and thus whether the user needs to be re-authenticated.
			//options.ValidationInterval : TimeSpan.Zero: The ValidationInterval property specifies how often the security stamp should be validated.
			//By setting this to TimeSpan.Zero, you’re instructing the validator to check the security stamp on every request.

			builder.Services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.Zero);


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthorization();
			var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
			using var scope = scopeFactory.CreateScope();

			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			await DefaultRoles.SeedRolesAsync(roleManager);
			await DefaultUsers.SeedAdminUserAsync(userManager);

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");
			app.MapRazorPages();

			app.Run();
		}
	}
}
