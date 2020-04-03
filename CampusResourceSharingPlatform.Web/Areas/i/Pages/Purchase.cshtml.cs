using System.Collections.Generic;
using System.Threading.Tasks;
using CampusResourceSharingPlatform.Interface;
using CampusResourceSharingPlatform.Model.Application;
using CampusResourceSharingPlatform.Model.Business;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CampusResourceSharingPlatform.Web.Areas.i.Pages
{
	public class PurchaseModel : PageModel
	{
		private readonly IPurchaseService<Purchase> _purchaseService;
		private readonly UserManager<ApplicationUser> _userManager;

		public PurchaseModel(IPurchaseService<Purchase> purchaseService,UserManager<ApplicationUser> userManager)
		{
			_purchaseService = purchaseService;
			_userManager = userManager;
		}

		public List<Purchase> PurchasePost { get; set; }

		[TempData]
		public string StatusMessage { get; set; }
		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			PurchasePost = await _purchaseService.GetAllActiveMissionByPostUserAsync(user);
			return Page();
		}
	}
}