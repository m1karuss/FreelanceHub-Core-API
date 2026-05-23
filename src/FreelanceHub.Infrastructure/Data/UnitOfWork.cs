using System;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Interfaces;
using FreelanceHub.Infrastructure.Data;
using FreelanceHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FreelanceHub.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Users = new UserRepository(_context);
            Projects = new ProjectRepository(_context);
            Bids = new BidRepository(_context);
            Payments = new PaymentRepository(_context);
            Messages = new MessageRepository(_context);
            Notifications = new NotificationRepository(_context);
            Reviews = new ReviewRepository(_context);
            UserActivities = new UserActivityRepository(_context);
            RefreshTokens = new RefreshTokenRepository(_context);
            FreelancerProfiles = new Repository<FreelancerProfile>(_context);
            ClientProfiles = new Repository<ClientProfile>(_context);
        }

        public IUserRepository Users { get; private set; }
        public IProjectRepository Projects { get; private set; }
        public IBidRepository Bids { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IMessageRepository Messages { get; private set; }
        public INotificationRepository Notifications { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IUserActivityRepository UserActivities { get; private set; }
        public IRefreshTokenRepository RefreshTokens { get; private set; }
        public IRepository<FreelancerProfile> FreelancerProfiles { get; private set; }
        public IRepository<ClientProfile> ClientProfiles { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
