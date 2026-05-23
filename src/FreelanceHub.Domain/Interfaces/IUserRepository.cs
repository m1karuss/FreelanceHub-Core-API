using System;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByEmailVerificationTokenAsync(string token);
        Task<User> GetByPasswordResetTokenAsync(string token);
        Task<bool> EmailExistsAsync(string email);
        Task<User> UpdateAsync(User user);
    }
}
