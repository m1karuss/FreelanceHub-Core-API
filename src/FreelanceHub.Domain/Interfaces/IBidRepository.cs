using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IBidRepository : IRepository<Bid>
    {
        Task<IEnumerable<Bid>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<Bid>> GetByFreelancerIdAsync(Guid freelancerId);
        Task<Bid> GetByProjectAndFreelancerAsync(Guid projectId, Guid freelancerId);
    }
}
