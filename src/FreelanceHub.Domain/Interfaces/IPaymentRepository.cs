using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<Payment>> GetBySenderIdAsync(Guid senderId);
        Task<IEnumerable<Payment>> GetByReceiverIdAsync(Guid receiverId);
        Task<Payment> GetByTransactionIdAsync(string transactionId);
    }
}
