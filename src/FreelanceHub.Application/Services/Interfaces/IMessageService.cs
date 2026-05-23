using FreelanceHub.Application.DTOs.Messages;

namespace FreelanceHub.Application.Services.Interfaces;

public interface IMessageService
{
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request);
    Task<MessageDto> GetMessageByIdAsync(Guid messageId, Guid userId);
    Task<IEnumerable<MessageDto>> GetConversationAsync(Guid user1Id, Guid user2Id);
    Task<IEnumerable<MessageDto>> GetInboxAsync(Guid userId);
    Task<IEnumerable<MessageDto>> GetSentAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid messageId, Guid userId);
    Task<bool> MarkAllAsReadAsync(Guid userId);
    Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
}
