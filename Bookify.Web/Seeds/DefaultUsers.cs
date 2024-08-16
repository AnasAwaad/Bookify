namespace Bookify.Web.Seeds;

public static class DefaultUsers
{
    public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {

        var admin = new ApplicationUser()
        {
            FullName = "Admin",
            UserName = "Admin",
            Email = "Admin@gmail.com",
            EmailConfirmed = true,
        };

        var user = await userManager.FindByEmailAsync(admin.Email);

        if (user is null)
        {
            await userManager.CreateAsync(admin, "P@ssword123");
            await userManager.AddToRoleAsync(admin, AppRoles.Admin);// assigned admin role to user
        }

    }
}
