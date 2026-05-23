namespace FreelanceHub.Application.DTOs.Payments;

public class CreatePaymentRequest
{
    public Guid ProjectId { get; set; }
    public Guid ReceiverId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Type { get; set; } = "ProjectPayment";
    public string? Description { get; set; }
    public Guid? MilestoneId { get; set; }
}
