using FreelanceHub.Application.DTOs.Reviews;

namespace FreelanceHub.Application.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateReviewAsync(Guid reviewerId, CreateReviewRequest request);
    Task<ReviewDto> GetReviewByIdAsync(Guid reviewId);
    Task<IEnumerable<ReviewDto>> GetReviewsByProjectAsync(Guid projectId);
    Task<IEnumerable<ReviewDto>> GetReviewsForUserAsync(Guid userId);
    Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid userId);
    Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId);
}
