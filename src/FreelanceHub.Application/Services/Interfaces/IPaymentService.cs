using FreelanceHub.Application.DTOs.Payments;

namespace FreelanceHub.Application.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentDto> CreatePaymentAsync(Guid senderId, CreatePaymentRequest request);
    Task<PaymentDto> GetPaymentByIdAsync(Guid paymentId, Guid userId);
    Task<IEnumerable<PaymentDto>> GetPaymentsByProjectAsync(Guid projectId);
    Task<IEnumerable<PaymentDto>> GetMyPaymentsAsync(Guid userId);
    Task<IEnumerable<PaymentDto>> GetMyReceivedPaymentsAsync(Guid userId);
    Task<bool> ProcessPaymentAsync(Guid paymentId);
    Task<bool> RefundPaymentAsync(Guid paymentId, Guid userId);
}
