using FreelanceHub.Application.DTOs.Reviews;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewDto> CreateReviewAsync(Guid reviewerId, CreateReviewRequest request)
    {
        var reviewer = await _unitOfWork.Users.GetByIdAsync(reviewerId)
            ?? throw new NotFoundException("Reviewer not found");

        var reviewee = await _unitOfWork.Users.GetByIdAsync(request.RevieweeId)
            ?? throw new NotFoundException("Reviewee not found");

        if (reviewerId == request.RevieweeId)
            throw new BadRequestException("Cannot review yourself");

        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId)
            ?? throw new NotFoundException($"Project with ID {request.ProjectId} not found");

        if (request.Rating < 1 || request.Rating > 5)
            throw new ValidationException(new[] { "Rating must be between 1 and 5" });

        // Verify reviewer is part of the project
        bool isClient = project.ClientId == reviewerId;
        bool isFreelancer = project.AwardedFreelancerId == reviewerId;

        if (!isClient && !isFreelancer)
            throw new ForbiddenException("You must be a participant of this project to leave a review");

        // Check for duplicate review
        var existing = await _unitOfWork.Reviews.GetByProjectIdAsync(request.ProjectId);
        if (existing.Any(r => r.ReviewerId == reviewerId && r.RevieweeId == request.RevieweeId))
            throw new ConflictException("You have already reviewed this user for this project");

        var review = new Review
        {
            ProjectId = request.ProjectId,
            ReviewerId = reviewerId,
            RevieweeId = request.RevieweeId,
            Rating = request.Rating,
            Comment = request.Comment,
            Pros = request.Pros,
            Cons = request.Cons,
            IsPublic = request.IsPublic,
            PublishedAt = request.IsPublic ? DateTime.UtcNow : null
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(review, project, reviewer, reviewee);
    }

    public async Task<ReviewDto> GetReviewByIdAsync(Guid reviewId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId)
            ?? throw new NotFoundException($"Review with ID {reviewId} not found");

        var project = await _unitOfWork.Projects.GetByIdAsync(review.ProjectId);
        var reviewer = await _unitOfWork.Users.GetByIdAsync(review.ReviewerId);
        var reviewee = await _unitOfWork.Users.GetByIdAsync(review.RevieweeId);

        return MapToDto(review, project, reviewer, reviewee);
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByProjectAsync(Guid projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        var reviews = await _unitOfWork.Reviews.GetByProjectIdAsync(projectId);

        var dtos = new List<ReviewDto>();
        foreach (var review in reviews)
        {
            var reviewer = await _unitOfWork.Users.GetByIdAsync(review.ReviewerId);
            var reviewee = await _unitOfWork.Users.GetByIdAsync(review.RevieweeId);
            dtos.Add(MapToDto(review, project, reviewer, reviewee));
        }

        return dtos;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsForUserAsync(Guid userId)
    {
        var reviews = await _unitOfWork.Reviews.GetByRevieweeIdAsync(userId);

        var dtos = new List<ReviewDto>();
        foreach (var review in reviews)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(review.ProjectId);
            var reviewer = await _unitOfWork.Users.GetByIdAsync(review.ReviewerId);
            var reviewee = await _unitOfWork.Users.GetByIdAsync(review.RevieweeId);
            dtos.Add(MapToDto(review, project, reviewer, reviewee));
        }

        return dtos;
    }

    public async Task<IEnumerable<ReviewDto>> GetReviewsByUserAsync(Guid userId)
    {
        var allReviews = await _unitOfWork.Reviews.GetByRevieweeIdAsync(userId);
        // IReviewRepository only has GetByRevieweeId; filter by reviewer from the full list
        // For given reviewer we get from project list
        var projects = await _unitOfWork.Projects.GetByClientIdAsync(userId);
        var projectIds = projects.Select(p => p.Id).ToHashSet();

        var dtos = new List<ReviewDto>();
        foreach (var review in allReviews.Where(r => r.ReviewerId == userId))
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(review.ProjectId);
            var reviewer = await _unitOfWork.Users.GetByIdAsync(review.ReviewerId);
            var reviewee = await _unitOfWork.Users.GetByIdAsync(review.RevieweeId);
            dtos.Add(MapToDto(review, project, reviewer, reviewee));
        }

        return dtos;
    }

    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId)
            ?? throw new NotFoundException($"Review with ID {reviewId} not found");

        if (review.ReviewerId != userId)
            throw new ForbiddenException("You can only delete your own reviews");

        _unitOfWork.Reviews.Remove(review);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static ReviewDto MapToDto(Review review, Project? project, User? reviewer, User? reviewee) => new()
    {
        Id = review.Id,
        ProjectId = review.ProjectId,
        ProjectTitle = project?.Title ?? string.Empty,
        ReviewerId = review.ReviewerId,
        ReviewerName = reviewer != null ? $"{reviewer.FirstName} {reviewer.LastName}" : string.Empty,
        RevieweeId = review.RevieweeId,
        RevieweeName = reviewee != null ? $"{reviewee.FirstName} {reviewee.LastName}" : string.Empty,
        Rating = review.Rating,
        Comment = review.Comment,
        Pros = review.Pros,
        Cons = review.Cons,
        IsPublic = review.IsPublic,
        PublishedAt = review.PublishedAt,
        CreatedAt = review.CreatedAt
    };
}
