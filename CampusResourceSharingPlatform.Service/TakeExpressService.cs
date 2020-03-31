﻿using CampusResourceSharingPlatform.Data;
using CampusResourceSharingPlatform.Interface;
using CampusResourceSharingPlatform.Model.Business;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusResourceSharingPlatform.Service
{
	public class TakeExpressService : ITakeExpressService<Express>
	{
		private readonly ApplicationDbContext _dbContext;

		public TakeExpressService(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}
		public int Post(Express newPost)
		{
			var result = _dbContext.MissionExpresses.AddAsync(newPost);
			if (!result.IsCompletedSuccessfully) return 0;
			_dbContext.SaveChanges();
			return 1;
		}

		public Task<int> PostAsync(Express newPost)
		{
			throw new NotImplementedException();
		}

		public int Update(Express newPost)
		{
			_dbContext.MissionExpresses.Update(newPost);
			_dbContext.SaveChanges();
			return 1;
		}

		public async Task<Express> GetLastMissionInfoAsync(string userId)
		{
			var post = await _dbContext.MissionExpresses.OrderByDescending(p => p.PostTime)
				.Where(p => p.PostUserId == userId).FirstOrDefaultAsync();
			return post;
		}

		public async Task<List<Express>> GetAllActiveMissionAsync()
		{
			var post = await _dbContext.MissionExpresses.OrderByDescending(p => p.PostTime).Where(p => p.InvalidTime > DateTime.UtcNow).ToListAsync();
			return post;
		}

		public async Task<List<Express>> GetTop10ActiveMissionAsync()
		{
			var post = await _dbContext.MissionExpresses.OrderByDescending(p => p.PostTime).Where(p => p.InvalidTime > DateTime.UtcNow).Take(10).ToListAsync();
			return post;
		}

		public async Task<Express> GetMissionById(string postId)
		{
			var post = await _dbContext.MissionExpresses.FindAsync(postId);
			return post;
		}
	}
}
