using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByRevieweeIdAsync(Guid revieweeId);
        Task<IEnumerable<Review>> GetByProjectIdAsync(Guid projectId);
        Task<double> GetAverageRatingAsync(Guid userId);
    }
}
