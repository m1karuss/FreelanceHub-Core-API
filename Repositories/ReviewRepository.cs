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
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByRevieweeIdAsync(Guid revieweeId)
        {
            return await _dbSet
                .Include(r => r.Reviewer)
                .Include(r => r.Project)
                .Where(r => r.RevieweeId == revieweeId && r.IsPublic)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByProjectIdAsync(Guid projectId)
        {
            return await _dbSet
                .Include(r => r.Reviewer)
                .Include(r => r.Reviewee)
                .Where(r => r.ProjectId == projectId)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingAsync(Guid userId)
        {
            var reviews = await _dbSet
                .Where(r => r.RevieweeId == userId && r.IsPublic)
                .ToListAsync();

            if (!reviews.Any())
                return 0;

            return reviews.Average(r => r.Rating);
        }
    }
}
