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
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetByProjectIdAsync(Guid projectId)
        {
            return await _dbSet
                .Include(p => p.Sender)
                .Include(p => p.Receiver)
                .Where(p => p.ProjectId == projectId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetBySenderIdAsync(Guid senderId)
        {
            return await _dbSet
                .Include(p => p.Receiver)
                .Include(p => p.Project)
                .Where(p => p.SenderId == senderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByReceiverIdAsync(Guid receiverId)
        {
            return await _dbSet
                .Include(p => p.Sender)
                .Include(p => p.Project)
                .Where(p => p.ReceiverId == receiverId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            return await _dbSet
                .Include(p => p.Sender)
                .Include(p => p.Receiver)
                .Include(p => p.Project)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);
        }
    }
}
