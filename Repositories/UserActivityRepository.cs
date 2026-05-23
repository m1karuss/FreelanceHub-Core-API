using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Interfaces;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories
{
    public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
    {
        public UserActivityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<UserActivity>> GetByUserIdAsync(Guid userId, int skip, int take)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }
    }
}
