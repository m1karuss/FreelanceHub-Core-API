using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IUserActivityRepository : IRepository<UserActivity>
    {
        Task<IEnumerable<UserActivity>> GetByUserIdAsync(Guid userId, int skip, int take);
    }
}
