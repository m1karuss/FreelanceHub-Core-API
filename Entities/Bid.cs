using System;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class Bid : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid FreelancerId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int DeliveryTime { get; set; }
        public string DeliveryTimeUnit { get; set; }
        public string CoverLetter { get; set; }
        public BidStatus Status { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public DateTime? RejectedAt { get; set; }
        public string RejectionReason { get; set; }

        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual User Freelancer { get; set; }
    }
}
