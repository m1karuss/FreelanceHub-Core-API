using System;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentType Type { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal NetAmount { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string ProcessorResponse { get; set; }
        public string Description { get; set; }
        public Guid? MilestoneId { get; set; }

        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual User Sender { get; set; }
        public virtual User Receiver { get; set; }
        public virtual Milestone Milestone { get; set; }
    }
}
