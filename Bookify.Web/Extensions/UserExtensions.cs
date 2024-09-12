namespace Bookify.Web.Extenstions;

public static class UserExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;
    }
}
