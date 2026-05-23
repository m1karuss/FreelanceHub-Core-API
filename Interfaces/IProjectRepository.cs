using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> GetByClientIdAsync(Guid clientId);
        Task<IEnumerable<Project>> GetByStatusAsync(ProjectStatus status);
        Task<IEnumerable<Project>> SearchProjectsAsync(string searchTerm, string category, decimal? minBudget, decimal? maxBudget, int skip, int take);
        Task<int> GetProjectsCountAsync(string searchTerm, string category, decimal? minBudget, decimal? maxBudget);
    }
}
