using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Notifications;
using FreelanceHub.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get all notifications for the current user
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<NotificationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications()
    {
        var result = await _notificationService.GetNotificationsAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<NotificationDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get unread notification count
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _notificationService.GetUnreadCountAsync(CurrentUserId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
    {
        var result = await _notificationService.MarkAsReadAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Notification marked as read"));
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPost("read-all")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _notificationService.MarkAllAsReadAsync(CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "All notifications marked as read"));
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteNotification([FromRoute] Guid id)
    {
        var result = await _notificationService.DeleteNotificationAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Notification deleted successfully"));
    }
}
