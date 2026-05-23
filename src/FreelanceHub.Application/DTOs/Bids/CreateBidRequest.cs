namespace FreelanceHub.Application.DTOs.Bids;

public class CreateBidRequest
{
    public Guid ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public int DeliveryTime { get; set; }
    public string DeliveryTimeUnit { get; set; } = "Days";
    public string CoverLetter { get; set; } = string.Empty;
}
