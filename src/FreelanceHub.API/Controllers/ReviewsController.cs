using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Reviews;
using FreelanceHub.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get all reviews for a specific project
    /// </summary>
    [HttpGet("project/{projectId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewsByProject([FromRoute] Guid projectId)
    {
        var result = await _reviewService.GetReviewsByProjectAsync(projectId);
        return Ok(ApiResponse<IEnumerable<ReviewDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get all reviews for a user (reviews about them)
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewsForUser([FromRoute] Guid userId)
    {
        var result = await _reviewService.GetReviewsForUserAsync(userId);
        return Ok(ApiResponse<IEnumerable<ReviewDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get a specific review by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReview([FromRoute] Guid id)
    {
        var result = await _reviewService.GetReviewByIdAsync(id);
        return Ok(ApiResponse<ReviewDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Create a review for a project participant
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReviewDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest request)
    {
        _logger.LogInformation("User {ReviewerId} creating review for user {RevieweeId} on project {ProjectId}",
            CurrentUserId, request.RevieweeId, request.ProjectId);

        var result = await _reviewService.CreateReviewAsync(CurrentUserId, request);

        return CreatedAtAction(nameof(GetReview), new { id = result.Id },
            ApiResponse<ReviewDto>.SuccessResponse(result, "Review created successfully"));
    }

    /// <summary>
    /// Delete a review (owner only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteReview([FromRoute] Guid id)
    {
        var result = await _reviewService.DeleteReviewAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Review deleted successfully"));
    }
}
