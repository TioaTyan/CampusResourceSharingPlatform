﻿using System.ComponentModel.DataAnnotations;

namespace CampusResourceSharingPlatform.Model.Business
{
	public class MissionBase
	{
		/// <summary>
		/// Id
		/// </summary>
		[Required]
		[MaxLength(64)]
		public string Id { get; set; }

		/// <summary>
		/// 删除标记
		/// </summary>
		[Required]
		public bool DeletedMark { get; set; }
	}
}
