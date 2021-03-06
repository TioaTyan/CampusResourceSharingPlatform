using CampusResourceSharingPlatform.Interface;
using CampusResourceSharingPlatform.Model.Application;
using CampusResourceSharingPlatform.Model.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CampusResourceSharingPlatform.Web.Areas.i.Pages
{
	[Authorize]
	public class TakeExpressModel : PageModel
	{
		private readonly IMissionService<Express> _takeExpressService;
		private readonly UserManager<ApplicationUser> _userManager;

		public TakeExpressModel(IMissionService<Express> takeExpressService, UserManager<ApplicationUser> userManager)
		{
			_takeExpressService = takeExpressService;
			_userManager = userManager;
		}
		public List<Express> ExpressPost { get; set; }

		[TempData]
		public string StatusMessage { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			ExpressPost = await _takeExpressService.GetAllActiveMissionByPostUserAsync(user);
			return Page();
		}
	}
}
