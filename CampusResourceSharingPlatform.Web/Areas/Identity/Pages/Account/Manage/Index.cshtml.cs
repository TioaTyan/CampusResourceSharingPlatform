﻿using CampusResourceSharingPlatform.Model.Application;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace CampusResourceSharingPlatform.Web.Areas.Identity.Pages.Account.Manage
{
	public partial class IndexModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IWebHostEnvironment _iWebHostEnvironment;

		public IndexModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IWebHostEnvironment iWebHostEnvironment)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_iWebHostEnvironment = iWebHostEnvironment;
		}

		public string Username { get; set; }
		public string AvatarFileName { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		[BindProperty]
		public AvatarModel Avatar { get; set; }

		[BindProperty]
		public NickNameModel NickName { get; set; }

		public class InputModel
		{
			[Phone]
			[Required]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }
		}
		public class AvatarModel
		{
			[Display(Name = "Avatar")]
			[Required]
			public IFormFile NewAvatar { get; set; }
		}

		public class NickNameModel
		{
			[Display(Name = "NickName")]
			public string NewNickName { get; set; }
		}

		private async Task LoadAsync(ApplicationUser user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			var nickName = user.NickName;

			Username = userName;

			Input = new InputModel
			{
				PhoneNumber = phoneNumber
			};

			NickName = new NickNameModel
			{
				NewNickName = nickName
			};
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}
			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAvatarAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			//判断用户是否存在
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			//判断表达合法
			if (Avatar.NewAvatar.Length == 0)
			{
				StatusMessage = "Your profile has no changes";
				return RedirectToPage();
			}
			var uploadFolder = Path.Combine(_iWebHostEnvironment.WebRootPath, "avatar", user.Id);
			AvatarFileName = Guid.NewGuid() + Path.GetExtension(Avatar.NewAvatar.FileName);
			var filePath = Path.Combine(uploadFolder, AvatarFileName);
			if (!Directory.Exists(uploadFolder))
			{
				Directory.CreateDirectory(uploadFolder);
			}
			await Avatar.NewAvatar.CopyToAsync(new FileStream(filePath, FileMode.Create));
			try
			{
				user.ProfilePhotoUrl = "/avatar/" + user.Id + "/" + AvatarFileName;
				var result = await _userManager.UpdateAsync(user);
				if (!result.Succeeded)
				{
					StatusMessage = "Error：图片上传失败，请重试。";
				}
				StatusMessage = "Success：图片上传成功。";
			}
			catch
			{
				StatusMessage = "Error：发生了不可预料到的错误，请重试。";
			}
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostNickNameAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			//判断用户是否存在
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			//判断表达合法
			if (string.IsNullOrEmpty(NickName.NewNickName))
			{
				StatusMessage = "Your profile has no changes";
				return RedirectToPage();
			}

			user.NickName = NickName.NewNickName;
			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var userId = await _userManager.GetUserIdAsync(user);
				throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}
