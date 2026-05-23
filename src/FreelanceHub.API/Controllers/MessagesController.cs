using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Messages;
using FreelanceHub.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get inbox (received messages)
    /// </summary>
    [HttpGet("inbox")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInbox()
    {
        var result = await _messageService.GetInboxAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<MessageDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get sent messages
    /// </summary>
    [HttpGet("sent")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSent()
    {
        var result = await _messageService.GetSentAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<MessageDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get conversation with a specific user
    /// </summary>
    [HttpGet("conversation/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversation([FromRoute] Guid userId)
    {
        var result = await _messageService.GetConversationAsync(CurrentUserId, userId);
        return Ok(ApiResponse<IEnumerable<MessageDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get unread message count
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var count = await _messageService.GetUnreadCountAsync(CurrentUserId);
        return Ok(ApiResponse<int>.SuccessResponse(count));
    }

    /// <summary>
    /// Get a specific message by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMessage([FromRoute] Guid id)
    {
        var result = await _messageService.GetMessageByIdAsync(id, CurrentUserId);
        return Ok(ApiResponse<MessageDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Send a message to another user
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        _logger.LogInformation("User {UserId} sending message to {ReceiverId}", CurrentUserId, request.ReceiverId);

        var result = await _messageService.SendMessageAsync(CurrentUserId, request);

        return CreatedAtAction(nameof(GetMessage), new { id = result.Id },
            ApiResponse<MessageDto>.SuccessResponse(result, "Message sent successfully"));
    }

    /// <summary>
    /// Mark a message as read
    /// </summary>
    [HttpPost("{id:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead([FromRoute] Guid id)
    {
        var result = await _messageService.MarkAsReadAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Message marked as read"));
    }

    /// <summary>
    /// Mark all messages as read
    /// </summary>
    [HttpPost("read-all")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var result = await _messageService.MarkAllAsReadAsync(CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "All messages marked as read"));
    }

    /// <summary>
    /// Delete a message
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteMessage([FromRoute] Guid id)
    {
        var result = await _messageService.DeleteMessageAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Message deleted successfully"));
    }
}
