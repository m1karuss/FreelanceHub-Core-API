using System;
using System.Threading.Tasks;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProjectRepository Projects { get; }
        IBidRepository Bids { get; }
        IPaymentRepository Payments { get; }
        IMessageRepository Messages { get; }
        INotificationRepository Notifications { get; }
        IReviewRepository Reviews { get; }
        IUserActivityRepository UserActivities { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
