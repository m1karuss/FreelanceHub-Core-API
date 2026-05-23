using FreelanceHub.Application.DTOs.Messages;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageRequest request)
    {
        var sender = await _unitOfWork.Users.GetByIdAsync(senderId)
            ?? throw new NotFoundException("Sender not found");

        var receiver = await _unitOfWork.Users.GetByIdAsync(request.ReceiverId)
            ?? throw new NotFoundException("Receiver not found");

        if (senderId == request.ReceiverId)
            throw new BadRequestException("Cannot send a message to yourself");

        if (request.ProjectId.HasValue)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId.Value);
            if (project == null)
                throw new NotFoundException($"Project with ID {request.ProjectId} not found");
        }

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            ProjectId = request.ProjectId,
            Subject = request.Subject,
            Body = request.Body,
            IsRead = false,
            ParentMessageId = request.ParentMessageId
        };

        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(message, sender, receiver);
    }

    public async Task<MessageDto> GetMessageByIdAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId)
            ?? throw new NotFoundException($"Message with ID {messageId} not found");

        if (message.SenderId != userId && message.ReceiverId != userId)
            throw new ForbiddenException("You are not authorized to view this message");

        var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId);
        var receiver = await _unitOfWork.Users.GetByIdAsync(message.ReceiverId);

        return MapToDto(message, sender, receiver);
    }

    public async Task<IEnumerable<MessageDto>> GetConversationAsync(Guid user1Id, Guid user2Id)
    {
        var messages = await _unitOfWork.Messages.GetConversationAsync(user1Id, user2Id);

        var user1 = await _unitOfWork.Users.GetByIdAsync(user1Id);
        var user2 = await _unitOfWork.Users.GetByIdAsync(user2Id);

        return messages.Select(m =>
        {
            var sender = m.SenderId == user1Id ? user1 : user2;
            var receiver = m.ReceiverId == user1Id ? user1 : user2;
            return MapToDto(m, sender, receiver);
        });
    }

    public async Task<IEnumerable<MessageDto>> GetInboxAsync(Guid userId)
    {
        var messages = await _unitOfWork.Messages.GetReceivedMessagesAsync(userId);

        var dtos = new List<MessageDto>();
        foreach (var message in messages)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId);
            var receiver = await _unitOfWork.Users.GetByIdAsync(message.ReceiverId);
            dtos.Add(MapToDto(message, sender, receiver));
        }

        return dtos;
    }

    public async Task<IEnumerable<MessageDto>> GetSentAsync(Guid userId)
    {
        var messages = await _unitOfWork.Messages.GetSentMessagesAsync(userId);

        var dtos = new List<MessageDto>();
        foreach (var message in messages)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(message.SenderId);
            var receiver = await _unitOfWork.Users.GetByIdAsync(message.ReceiverId);
            dtos.Add(MapToDto(message, sender, receiver));
        }

        return dtos;
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _unitOfWork.Messages.GetUnreadCountAsync(userId);
    }

    public async Task<bool> MarkAsReadAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId)
            ?? throw new NotFoundException($"Message with ID {messageId} not found");

        if (message.ReceiverId != userId)
            throw new ForbiddenException("You can only mark your own received messages as read");

        if (message.IsRead)
            return true;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _unitOfWork.Messages.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId)
    {
        var messages = await _unitOfWork.Messages.GetReceivedMessagesAsync(userId);
        var unreadMessages = messages.Where(m => !m.IsRead).ToList();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Messages.UpdateAsync(message);
        }

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId)
            ?? throw new NotFoundException($"Message with ID {messageId} not found");

        if (message.SenderId != userId && message.ReceiverId != userId)
            throw new ForbiddenException("You are not authorized to delete this message");

        _unitOfWork.Messages.Remove(message);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static MessageDto MapToDto(Message message, User? sender, User? receiver) => new()
    {
        Id = message.Id,
        SenderId = message.SenderId,
        SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : string.Empty,
        ReceiverId = message.ReceiverId,
        ReceiverName = receiver != null ? $"{receiver.FirstName} {receiver.LastName}" : string.Empty,
        ProjectId = message.ProjectId,
        Subject = message.Subject,
        Body = message.Body,
        IsRead = message.IsRead,
        ReadAt = message.ReadAt,
        ParentMessageId = message.ParentMessageId,
        CreatedAt = message.CreatedAt
    };
}
