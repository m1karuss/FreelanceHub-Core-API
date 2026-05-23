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
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            return await _dbSet
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == token);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId && !r.IsRevoked && r.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeAllUserTokensAsync(Guid userId)
        {
            var tokens = await _dbSet
                .Where(r => r.UserId == userId && !r.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }
        }
    }
}
