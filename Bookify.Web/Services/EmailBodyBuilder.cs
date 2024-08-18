using Microsoft.AspNetCore.Routing.Template;
using System.Text.Encodings.Web;

namespace Bookify.Web.Services;

public class EmailBodyBuilder : IEmailBodyBuilder
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	public EmailBodyBuilder(IWebHostEnvironment webHostEnvironment)
	{
		_webHostEnvironment = webHostEnvironment;
	}


	public string GetEmailBody(string imageUrl, string imageLogo, string header, string body, string url, string linkTitle)
	{
		var templatePath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";

		StreamReader streamReader = new StreamReader(templatePath);
		var template = streamReader.ReadToEnd();
		streamReader.Close();

		return template.Replace("[imageUrl]", imageUrl)
			.Replace("[imageLogo]", imageLogo)
			.Replace("[header]", header)
			.Replace("[body]", body)
			.Replace("[url]", url)
			.Replace("[linkTitle]", linkTitle);

	}
}
