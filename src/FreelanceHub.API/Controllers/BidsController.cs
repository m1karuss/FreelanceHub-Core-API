using FreelanceHub.Application.DTOs.Bids;
using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BidsController : ControllerBase
{
    private readonly IBidService _bidService;
    private readonly ILogger<BidsController> _logger;

    public BidsController(IBidService bidService, ILogger<BidsController> logger)
    {
        _bidService = bidService;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get all bids for a specific project
    /// </summary>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BidDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBidsByProject([FromRoute] Guid projectId)
    {
        var result = await _bidService.GetBidsByProjectAsync(projectId);
        return Ok(ApiResponse<IEnumerable<BidDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get my submitted bids (freelancer)
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<BidDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBids()
    {
        var result = await _bidService.GetMyBidsAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<BidDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get a specific bid by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BidDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBid([FromRoute] Guid id)
    {
        var result = await _bidService.GetBidByIdAsync(id);
        return Ok(ApiResponse<BidDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Submit a bid on a project (freelancer only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BidDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitBid([FromBody] CreateBidRequest request)
    {
        _logger.LogInformation("Freelancer {FreelancerId} submitting bid on project {ProjectId}",
            CurrentUserId, request.ProjectId);

        var result = await _bidService.SubmitBidAsync(CurrentUserId, request);

        return CreatedAtAction(nameof(GetBid), new { id = result.Id },
            ApiResponse<BidDto>.SuccessResponse(result, "Bid submitted successfully"));
    }

    /// <summary>
    /// Update a pending bid (freelancer only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<BidDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateBid([FromRoute] Guid id, [FromBody] UpdateBidRequest request)
    {
        var result = await _bidService.UpdateBidAsync(id, CurrentUserId, request);
        return Ok(ApiResponse<BidDto>.SuccessResponse(result, "Bid updated successfully"));
    }

    /// <summary>
    /// Withdraw a pending bid (freelancer only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> WithdrawBid([FromRoute] Guid id)
    {
        var result = await _bidService.WithdrawBidAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Bid withdrawn successfully"));
    }

    /// <summary>
    /// Accept a bid (project owner/client only)
    /// </summary>
    [HttpPost("{id:guid}/accept")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AcceptBid([FromRoute] Guid id)
    {
        var result = await _bidService.AcceptBidAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Bid accepted successfully"));
    }

    /// <summary>
    /// Reject a bid (project owner/client only)
    /// </summary>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RejectBid([FromRoute] Guid id, [FromBody] RejectBidRequest request)
    {
        var result = await _bidService.RejectBidAsync(id, CurrentUserId, request.Reason);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Bid rejected successfully"));
    }
}

public class RejectBidRequest
{
    public string Reason { get; set; } = string.Empty;
}
