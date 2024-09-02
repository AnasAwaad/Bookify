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


	public string GetEmailBody(string templateName,Dictionary<string , string> placeholders)
	{
		var templatePath = $"{_webHostEnvironment.WebRootPath}/templates/{templateName}.html";

		StreamReader streamReader = new StreamReader(templatePath);
		var template = streamReader.ReadToEnd();
		streamReader.Close();

        foreach (var placeholder in placeholders)
        {
			template = template.Replace($"[{placeholder.Key}]", placeholder.Value);
        }

		return template;
    }
}
