using FreelanceHub.Application.DTOs.Common;
using FreelanceHub.Application.DTOs.Payments;
using FreelanceHub.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    /// Get payments I initiated (as sender)
    /// </summary>
    [HttpGet("sent")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPayments()
    {
        var result = await _paymentService.GetMyPaymentsAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get payments I received
    /// </summary>
    [HttpGet("received")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReceivedPayments()
    {
        var result = await _paymentService.GetMyReceivedPaymentsAsync(CurrentUserId);
        return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get all payments for a specific project
    /// </summary>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByProject([FromRoute] Guid projectId)
    {
        var result = await _paymentService.GetPaymentsByProjectAsync(projectId);
        return Ok(ApiResponse<IEnumerable<PaymentDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// Get a specific payment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment([FromRoute] Guid id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id, CurrentUserId);
        return Ok(ApiResponse<PaymentDto>.SuccessResponse(result));
    }

    /// <summary>
    /// Create a new payment (client only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
    {
        _logger.LogInformation("Client {ClientId} creating payment for project {ProjectId}",
            CurrentUserId, request.ProjectId);

        var result = await _paymentService.CreatePaymentAsync(CurrentUserId, request);

        return CreatedAtAction(nameof(GetPayment), new { id = result.Id },
            ApiResponse<PaymentDto>.SuccessResponse(result, "Payment created successfully"));
    }

    /// <summary>
    /// Process a pending payment
    /// </summary>
    [HttpPost("{id:guid}/process")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPayment([FromRoute] Guid id)
    {
        var result = await _paymentService.ProcessPaymentAsync(id);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Payment processed successfully"));
    }

    /// <summary>
    /// Request a refund for a completed payment
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefundPayment([FromRoute] Guid id)
    {
        var result = await _paymentService.RefundPaymentAsync(id, CurrentUserId);
        return Ok(ApiResponse<bool>.SuccessResponse(result, "Payment refunded successfully"));
    }
}
