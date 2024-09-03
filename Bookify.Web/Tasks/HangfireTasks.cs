using Bookify.Web.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;

namespace Bookify.Web.Tasks;

public class HangfireTasks
{
	private readonly ApplicationDbContext _context;
	private readonly IWhatsAppClient _whatsAppClient;
	private readonly IWebHostEnvironment _webHostEnvironment;
	private readonly IEmailBodyBuilder _emailBodyBuilder;
	private readonly IEmailSender _emailSender;

	public HangfireTasks(ApplicationDbContext context, IWhatsAppClient whatsAppClient, IWebHostEnvironment webHostEnvironment, IEmailBodyBuilder emailBodyBuilder,IEmailSender emailSender)
	{
		_context = context;
		_whatsAppClient = whatsAppClient;
		_webHostEnvironment = webHostEnvironment;
		_emailBodyBuilder = emailBodyBuilder;
		_emailSender = emailSender;
	}

	public async Task PrepareExpirationAlert()
	{
		var subscripers = _context.Subscripers
			.Include(s => s.Subscriptions)
			.Where(s => s.Subscriptions.OrderBy(s => s.EndDate).Last().EndDate == DateTime.Today.AddDays(7))
			.ToList();

		foreach (var subscriper in subscripers)
		{
			if (subscriper.HasWhatsApp)
			{
				var component = new List<WhatsAppComponent>()
				{
					new WhatsAppComponent
					{
						Type="body",
						Parameters=new List<object>
						{
							new WhatsAppTextParameter
							{
								Text=subscriper.FirstName
							},
							new WhatsAppTextParameter
							{
								Text=subscriper.Subscriptions.OrderBy(s=>s.EndDate).Last().EndDate.ToString("d MMM, YYYY")
							}
						}
					}
				};

				var mobileNumber = _webHostEnvironment.IsDevelopment() ? "201067873327" : $"2{subscriper.MobileNumber}";

				await _whatsAppClient.SendMessage(mobileNumber, WhatsAppLanguageCode.English_US, WhatsAppTemplates.HelloWorld);
			}

			// send welcome message to email user
			var placeholders = new Dictionary<string, string>()
			{
				{"imageUrl","https://res.cloudinary.com/dygrlijla/image/upload/v1723914644/93ae73da-0ad6-4e76-ad40-3ab67cf4c6f1.png" },
				{"imageLogo","https://res.cloudinary.com/dygrlijla/image/upload/v1723914720/logo_nh8slr.png" },
				{"header", $"Hey {subscriper.FirstName}" },
				{"body",$"your subscription will be expired by {subscriper.Subscriptions.OrderBy(s=>s.EndDate).Last().EndDate.ToString("d MMM,yyyy")}" },
			};


			var body = _emailBodyBuilder.GetEmailBody(EmailTemplates.Notification, placeholders);

			await _emailSender.SendEmailAsync(subscriper.Email, "Expired subscription", body);
		}
	}

}
