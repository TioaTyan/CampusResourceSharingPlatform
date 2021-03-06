﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CampusResourceSharingPlatform.Web.Areas.ControlPanel.Pages.Manage
{
	public static class ManageNavPages
	{
		public static string Index => "Index";

		public static string RoleIndex => "RoleIndex";

		public static string UsersIndex => "UsersIndex";

		public static string PostIndex => "PostIndex";

		public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
		public static string RoleIndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, RoleIndex);
		public static string UsersIndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, UsersIndex);
		public static string PostIndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, PostIndex);

		private static string PageNavClass(ViewContext viewContext, string page)
		{
			var activePage = viewContext.ViewData["ActivePage"] as string
				?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
			return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
		}
	}
}
