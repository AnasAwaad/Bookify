namespace Bookify.Web.Services;

public interface IEmailBodyBuilder
{
	public string GetEmailBody(string templateName, Dictionary<string, string> placeholders);
}
