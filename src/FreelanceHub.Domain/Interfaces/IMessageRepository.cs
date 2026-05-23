using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreelanceHub.Domain.Entities;

namespace FreelanceHub.Domain.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<IEnumerable<Message>> GetConversationAsync(Guid user1Id, Guid user2Id);
        Task<IEnumerable<Message>> GetReceivedMessagesAsync(Guid userId);
        Task<IEnumerable<Message>> GetSentMessagesAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}
