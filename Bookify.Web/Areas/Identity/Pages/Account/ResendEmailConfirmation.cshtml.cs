// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Text;
using System.Text.Encodings.Web;

namespace Bookify.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;

		public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment)
		{
			_userManager = userManager;
			_emailSender = emailSender;
			_webHostEnvironment = webHostEnvironment;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Email { get; set; }
        }

        public void OnGet(string email)
        {
            Input = new InputModel
            {
                Email = email
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

			var templatePath = $"{_webHostEnvironment.WebRootPath}/templates/email.html";

			StreamReader streamReader = new StreamReader(templatePath);
			var body = streamReader.ReadToEnd();
			streamReader.Close();

			body = body.Replace("[imageUrl]", "https://res.cloudinary.com/dygrlijla/image/upload/v1723914644/93ae73da-0ad6-4e76-ad40-3ab67cf4c6f1.png")
				.Replace("[imageLogo]", "https://res.cloudinary.com/dygrlijla/image/upload/v1723914720/logo_nh8slr.png")
				.Replace("[header]", $"Hey {user.FullName}, thanks for joining up!")
				.Replace("[body]", "Please confirm your email")
				.Replace("[url]", $"{HtmlEncoder.Default.Encode(callbackUrl!)}")
				.Replace("[linkTitle]", "Active Account!");

			await _emailSender.SendEmailAsync(
                Input.Email,
                "Confirm your email",body);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
