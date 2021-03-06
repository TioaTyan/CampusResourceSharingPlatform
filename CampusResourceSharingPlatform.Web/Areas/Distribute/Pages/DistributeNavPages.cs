﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CampusResourceSharingPlatform.Web.Areas.Distribute.Pages
{
	public static class DistributeNavPages
	{
		public static string TakeExpress => "TakeExpress";

		public static string Hire => "Hire";

		public static string Purchase => "Purchase";

		public static string Sale => "Sale";

		public static string TakeExpressNavClass(ViewContext viewContext) => PageNavClass(viewContext, TakeExpress);
		public static string HireNavClass(ViewContext viewContext) => PageNavClass(viewContext, Hire);

		public static string PurchaseNavClass(ViewContext viewContext) => PageNavClass(viewContext, Purchase);

		public static string SaleNavClass(ViewContext viewContext) => PageNavClass(viewContext, Sale);

		private static string PageNavClass(ViewContext viewContext, string page)
		{
			var activePage = viewContext.ViewData["ActivePage"] as string
							 ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
			return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
		}
	}
}
