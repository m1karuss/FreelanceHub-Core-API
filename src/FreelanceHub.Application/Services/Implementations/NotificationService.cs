using FreelanceHub.Application.DTOs.Notifications;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
        return notifications.Select(MapToDto);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _unitOfWork.Notifications.GetUnreadCountAsync(userId);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId)
            ?? throw new NotFoundException($"Notification with ID {notificationId} not found");

        if (notification.UserId != userId)
            throw new ForbiddenException("You are not authorized to update this notification");

        if (notification.IsRead)
            return true;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _unitOfWork.Notifications.UpdateAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(Guid userId)
    {
        var notifications = await _unitOfWork.Notifications.GetByUserIdAsync(userId);
        var unread = notifications.Where(n => !n.IsRead).ToList();

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _unitOfWork.Notifications.UpdateAsync(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId)
            ?? throw new NotFoundException($"Notification with ID {notificationId} not found");

        if (notification.UserId != userId)
            throw new ForbiddenException("You are not authorized to delete this notification");

        _unitOfWork.Notifications.Remove(notification);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task CreateNotificationAsync(Guid userId, string type, string title, string message, string? actionUrl = null)
    {
        if (!Enum.TryParse<NotificationType>(type, ignoreCase: true, out var notificationType))
            notificationType = NotificationType.System;

        var notification = new Notification
        {
            UserId = userId,
            Type = notificationType,
            Title = title,
            Message = message,
            IsRead = false,
            ActionUrl = actionUrl
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();
    }

    private static NotificationDto MapToDto(Notification notification) => new()
    {
        Id = notification.Id,
        UserId = notification.UserId,
        Type = notification.Type.ToString(),
        Title = notification.Title,
        Message = notification.Message,
        IsRead = notification.IsRead,
        ReadAt = notification.ReadAt,
        ActionUrl = notification.ActionUrl,
        CreatedAt = notification.CreatedAt
    };
}
