namespace FreelanceHub.Application.DTOs.Bids;

public class UpdateBidRequest
{
    public decimal? Amount { get; set; }
    public int? DeliveryTime { get; set; }
    public string? DeliveryTimeUnit { get; set; }
    public string? CoverLetter { get; set; }
}
