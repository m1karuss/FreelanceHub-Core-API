namespace FreelanceHub.Application.DTOs.Bids;

public class BidDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public Guid FreelancerId { get; set; }
    public string FreelancerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public int DeliveryTime { get; set; }
    public string DeliveryTimeUnit { get; set; } = string.Empty;
    public string CoverLetter { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? AcceptedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
