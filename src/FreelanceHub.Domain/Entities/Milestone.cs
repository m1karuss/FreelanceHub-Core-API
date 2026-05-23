using System;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class Milestone : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime DueDate { get; set; }
        public MilestoneStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int OrderIndex { get; set; }

        // Navigation property
        public virtual Project Project { get; set; }
    }
}
