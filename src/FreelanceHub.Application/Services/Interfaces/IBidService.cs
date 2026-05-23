using FreelanceHub.Application.DTOs.Bids;

namespace FreelanceHub.Application.Services.Interfaces;

public interface IBidService
{
    Task<BidDto> SubmitBidAsync(Guid freelancerId, CreateBidRequest request);
    Task<BidDto> GetBidByIdAsync(Guid bidId);
    Task<IEnumerable<BidDto>> GetBidsByProjectAsync(Guid projectId);
    Task<IEnumerable<BidDto>> GetMyBidsAsync(Guid freelancerId);
    Task<BidDto> UpdateBidAsync(Guid bidId, Guid freelancerId, UpdateBidRequest request);
    Task<bool> WithdrawBidAsync(Guid bidId, Guid freelancerId);
    Task<bool> AcceptBidAsync(Guid bidId, Guid clientId);
    Task<bool> RejectBidAsync(Guid bidId, Guid clientId, string reason);
}
