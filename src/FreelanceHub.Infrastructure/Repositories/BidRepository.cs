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
    public class BidRepository : Repository<Bid>, IBidRepository
    {
        public BidRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bid>> GetByProjectIdAsync(Guid projectId)
        {
            return await _dbSet
                .Include(b => b.Freelancer)
                    .ThenInclude(f => f.FreelancerProfile)
                .Where(b => b.ProjectId == projectId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bid>> GetByFreelancerIdAsync(Guid freelancerId)
        {
            return await _dbSet
                .Include(b => b.Project)
                    .ThenInclude(p => p.Client)
                .Where(b => b.FreelancerId == freelancerId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<Bid> GetByProjectAndFreelancerAsync(Guid projectId, Guid freelancerId)
        {
            return await _dbSet
                .Include(b => b.Project)
                .Include(b => b.Freelancer)
                .FirstOrDefaultAsync(b => b.ProjectId == projectId && b.FreelancerId == freelancerId);
        }
    }
}
