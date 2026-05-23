using FreelanceHub.Application.DTOs.Notifications;

namespace FreelanceHub.Application.Services.Interfaces;

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetNotificationsAsync(Guid userId);
    Task<int> GetUnreadCountAsync(Guid userId);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId);
    Task<bool> MarkAllAsReadAsync(Guid userId);
    Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);
    Task CreateNotificationAsync(Guid userId, string type, string title, string message, string? actionUrl = null);
}
