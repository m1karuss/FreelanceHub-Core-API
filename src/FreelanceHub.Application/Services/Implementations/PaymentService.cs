using FreelanceHub.Application.DTOs.Payments;
using FreelanceHub.Application.Exceptions;
using FreelanceHub.Application.Services.Interfaces;
using FreelanceHub.Domain.Entities;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Interfaces;

namespace FreelanceHub.Application.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private const decimal PlatformFeePercentage = 0.10m; // 10% platform fee

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentDto> CreatePaymentAsync(Guid senderId, CreatePaymentRequest request)
    {
        var sender = await _unitOfWork.Users.GetByIdAsync(senderId)
            ?? throw new NotFoundException("Sender not found");

        var receiver = await _unitOfWork.Users.GetByIdAsync(request.ReceiverId)
            ?? throw new NotFoundException("Receiver not found");

        var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId)
            ?? throw new NotFoundException($"Project with ID {request.ProjectId} not found");

        if (project.ClientId != senderId)
            throw new ForbiddenException("Only the project client can initiate payments");

        if (project.AwardedFreelancerId != request.ReceiverId)
            throw new BadRequestException("Payment receiver must be the awarded freelancer");

        if (!Enum.TryParse<PaymentType>(request.Type, ignoreCase: true, out var paymentType))
            throw new ValidationException(new[] { $"Invalid payment type: '{request.Type}'" });

        var platformFee = Math.Round(request.Amount * PlatformFeePercentage, 2);
        var netAmount = request.Amount - platformFee;

        var payment = new Payment
        {
            ProjectId = request.ProjectId,
            SenderId = senderId,
            ReceiverId = request.ReceiverId,
            Amount = request.Amount,
            Currency = request.Currency,
            Status = PaymentStatus.Pending,
            Type = paymentType,
            TransactionId = GenerateTransactionId(),
            PaymentMethod = request.PaymentMethod,
            PlatformFee = platformFee,
            NetAmount = netAmount,
            Description = request.Description,
            MilestoneId = request.MilestoneId
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(payment, project, sender, receiver);
    }

    public async Task<PaymentDto> GetPaymentByIdAsync(Guid paymentId, Guid userId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
            ?? throw new NotFoundException($"Payment with ID {paymentId} not found");

        if (payment.SenderId != userId && payment.ReceiverId != userId)
            throw new ForbiddenException("You are not authorized to view this payment");

        var project = await _unitOfWork.Projects.GetByIdAsync(payment.ProjectId);
        var sender = await _unitOfWork.Users.GetByIdAsync(payment.SenderId);
        var receiver = await _unitOfWork.Users.GetByIdAsync(payment.ReceiverId);

        return MapToDto(payment, project, sender, receiver);
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByProjectAsync(Guid projectId)
    {
        var project = await _unitOfWork.Projects.GetByIdAsync(projectId)
            ?? throw new NotFoundException($"Project with ID {projectId} not found");

        var payments = await _unitOfWork.Payments.GetByProjectIdAsync(projectId);

        var dtos = new List<PaymentDto>();
        foreach (var payment in payments)
        {
            var sender = await _unitOfWork.Users.GetByIdAsync(payment.SenderId);
            var receiver = await _unitOfWork.Users.GetByIdAsync(payment.ReceiverId);
            dtos.Add(MapToDto(payment, project, sender, receiver));
        }

        return dtos;
    }

    public async Task<IEnumerable<PaymentDto>> GetMyPaymentsAsync(Guid userId)
    {
        var payments = await _unitOfWork.Payments.GetBySenderIdAsync(userId);

        var dtos = new List<PaymentDto>();
        foreach (var payment in payments)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(payment.ProjectId);
            var sender = await _unitOfWork.Users.GetByIdAsync(payment.SenderId);
            var receiver = await _unitOfWork.Users.GetByIdAsync(payment.ReceiverId);
            dtos.Add(MapToDto(payment, project, sender, receiver));
        }

        return dtos;
    }

    public async Task<IEnumerable<PaymentDto>> GetMyReceivedPaymentsAsync(Guid userId)
    {
        var payments = await _unitOfWork.Payments.GetByReceiverIdAsync(userId);

        var dtos = new List<PaymentDto>();
        foreach (var payment in payments)
        {
            var project = await _unitOfWork.Projects.GetByIdAsync(payment.ProjectId);
            var sender = await _unitOfWork.Users.GetByIdAsync(payment.SenderId);
            var receiver = await _unitOfWork.Users.GetByIdAsync(payment.ReceiverId);
            dtos.Add(MapToDto(payment, project, sender, receiver));
        }

        return dtos;
    }

    public async Task<bool> ProcessPaymentAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
            ?? throw new NotFoundException($"Payment with ID {paymentId} not found");

        if (payment.Status != PaymentStatus.Pending)
            throw new BadRequestException("Only pending payments can be processed");

        // In production: integrate with Stripe/PayPal here
        payment.Status = PaymentStatus.Completed;
        payment.ProcessedAt = DateTime.UtcNow;
        payment.ProcessorResponse = "Payment processed successfully";

        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RefundPaymentAsync(Guid paymentId, Guid userId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId)
            ?? throw new NotFoundException($"Payment with ID {paymentId} not found");

        if (payment.SenderId != userId)
            throw new ForbiddenException("Only the payment sender can request a refund");

        if (payment.Status != PaymentStatus.Completed)
            throw new BadRequestException("Only completed payments can be refunded");

        payment.Status = PaymentStatus.Refunded;
        payment.ProcessorResponse = "Refunded";

        await _unitOfWork.Payments.UpdateAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private static string GenerateTransactionId()
        => $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";

    private static PaymentDto MapToDto(Payment payment, Project? project, User? sender, User? receiver) => new()
    {
        Id = payment.Id,
        ProjectId = payment.ProjectId,
        ProjectTitle = project?.Title ?? string.Empty,
        SenderId = payment.SenderId,
        SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : string.Empty,
        ReceiverId = payment.ReceiverId,
        ReceiverName = receiver != null ? $"{receiver.FirstName} {receiver.LastName}" : string.Empty,
        Amount = payment.Amount,
        Currency = payment.Currency,
        Status = payment.Status.ToString(),
        Type = payment.Type.ToString(),
        TransactionId = payment.TransactionId,
        PaymentMethod = payment.PaymentMethod,
        PlatformFee = payment.PlatformFee,
        NetAmount = payment.NetAmount,
        Description = payment.Description,
        MilestoneId = payment.MilestoneId,
        ProcessedAt = payment.ProcessedAt,
        CreatedAt = payment.CreatedAt
    };
}
