namespace Bookify.Web.Services;

public interface IEmailBodyBuilder
{
	public string GetEmailBody(string imageUrl, string imageLogo, string header, string body, string url, string linkTitle);
}
