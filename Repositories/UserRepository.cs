using System;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Interfaces;
using FreelanceHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.FreelancerProfile)
                .Include(u => u.ClientProfile)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByEmailVerificationTokenAsync(string token)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.EmailVerificationToken == token
                    && u.EmailVerificationTokenExpiry > DateTime.UtcNow);
        }

        public async Task<User> GetByPasswordResetTokenAsync(string token)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token
                    && u.PasswordResetTokenExpiry > DateTime.UtcNow);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email);
        }

        public override async Task<User> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(u => u.FreelancerProfile)
                .Include(u => u.ClientProfile)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
