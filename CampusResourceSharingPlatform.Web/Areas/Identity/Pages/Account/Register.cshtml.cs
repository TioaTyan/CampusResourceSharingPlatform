﻿using CampusResourceSharingPlatform.Model.Application;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CampusResourceSharingPlatform.Web.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public RegisterModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class InputModel
		{
			[Required(ErrorMessage = "用户名为必填项。")]
			[Display(Name = "用户名")]
			[PageRemote(PageHandler = "CheckUserNameExist", HttpMethod = "Get", ErrorMessage = "用户名已存在")]
			public string UserName { get; set; }

			[Required(ErrorMessage = "邮箱地址为必填项。")]
			[EmailAddress]
			[Display(Name = "邮箱地址")]
			[PageRemote(PageHandler = "CheckUserEmailExist", HttpMethod = "Get", ErrorMessage = "用户名已存在")]
			public string Email { get; set; }

			[Required(ErrorMessage = "密码为必填项")]
			[StringLength(100, ErrorMessage = "{0}的长度应该控制在{2}-{1}之间。", MinimumLength = 6)]
			[DataType(DataType.Password)]
			[Display(Name = "密码")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "重复密码")]
			[Compare("Password", ErrorMessage = "两次密码输入不一致，请重新检查输入。")]
			public string ConfirmPassword { get; set; }
		}

		public async Task OnGetAsync(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (!ModelState.IsValid)
			{
				return Page();
			}
			var user = new ApplicationUser
			{
				UserName = Input.UserName,
				NickName = Input.UserName,
				Email = Input.Email,
				ProfilePhotoUrl = "/avatar/default.jpg",
			};
			var result = await _userManager.CreateAsync(user, Input.Password);
			if (result.Succeeded)
			{
				_logger.LogInformation("User created a new account with password.");

				var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				var callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					pageHandler: null,
					values: new { area = "Identity", userId = user.Id, code = code },
					protocol: Request.Scheme);
				await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
					$"<h3>欢迎使用校园闲散资源共享平台</h3>" +
					$"请<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>点击这里</a>验证你的邮箱。<br />" +
					$" 如果你的浏览器不支持跳转，请复制下面的网址到浏览器地址栏进行手动跳转。" +
					$"<br /><br /> {callbackUrl}");

				if (_userManager.Options.SignIn.RequireConfirmedAccount)
				{
					return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
				}
				else
				{
					await _signInManager.SignInAsync(user, isPersistent: false);
					return LocalRedirect(returnUrl);
				}
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
			return Page();
		}

		public async Task<IActionResult> OnGetCheckUserNameExist(InputModel input)
		{
			var user = await _userManager.FindByNameAsync(input.UserName);
			return user != null ? new JsonResult($"用户名{input.UserName}已存在") : new JsonResult(true);
		}

		public async Task<IActionResult> OnGetCheckUserEmailExist(InputModel input)
		{
			var user = await _userManager.FindByEmailAsync(input.Email);
			return user != null ? new JsonResult($"邮箱{input.Email}已被注册") : new JsonResult(true);
		}
	}
}
