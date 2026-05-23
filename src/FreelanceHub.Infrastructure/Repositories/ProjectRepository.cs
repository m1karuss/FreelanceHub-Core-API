using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Interfaces;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetByClientIdAsync(Guid clientId)
        {
            return await _dbSet
                .Include(p => p.Client)
                .Include(p => p.Bids)
                .Where(p => p.ClientId == clientId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status)
        {
            return await _dbSet
                .Include(p => p.Client)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PublishedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(
            string searchTerm,
            string category,
            decimal? minBudget,
            decimal? maxBudget,
            int skip,
            int take)
        {
            var query = _dbSet
                .Include(p => p.Client)
                .Where(p => p.Status == ProjectStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Skills.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (minBudget.HasValue)
            {
                query = query.Where(p => p.Budget >= minBudget.Value);
            }

            if (maxBudget.HasValue)
            {
                query = query.Where(p => p.Budget <= maxBudget.Value);
            }

            return await query
                .OrderByDescending(p => p.PublishedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetProjectsCountAsync(
            string searchTerm,
            string category,
            decimal? minBudget,
            decimal? maxBudget)
        {
            var query = _dbSet
                .Where(p => p.Status == ProjectStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Skills.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (minBudget.HasValue)
            {
                query = query.Where(p => p.Budget >= minBudget.Value);
            }

            if (maxBudget.HasValue)
            {
                query = query.Where(p => p.Budget <= maxBudget.Value);
            }

            return await query.CountAsync();
        }

        public override async Task<Project> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Client)
                .Include(p => p.Bids)
                    .ThenInclude(b => b.Freelancer)
                .Include(p => p.Milestones)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
