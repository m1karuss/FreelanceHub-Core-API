namespace FreelanceHub.Application.DTOs.Messages;

public class SendMessageRequest
{
    public Guid ReceiverId { get; set; }
    public Guid? ProjectId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public Guid? ParentMessageId { get; set; }
}
