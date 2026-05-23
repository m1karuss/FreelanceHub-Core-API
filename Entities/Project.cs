using System;
using System.Collections.Generic;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class Project : BaseEntity
    {
        public Guid ClientId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Skills { get; set; }
        public ProjectStatus Status { get; set; }
        public ProjectType Type { get; set; }
        public decimal Budget { get; set; }
        public string Currency { get; set; }
        public DateTime? Deadline { get; set; }
        public int EstimatedDuration { get; set; }
        public string DurationUnit { get; set; }
        public ExperienceLevel RequiredExperienceLevel { get; set; }
        public int ViewsCount { get; set; }
        public int BidsCount { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public Guid? AwardedFreelancerId { get; set; }
        public DateTime? AwardedAt { get; set; }
        public string Attachments { get; set; }

        // Navigation properties
        public virtual User Client { get; set; }
        public virtual User AwardedFreelancer { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Milestone> Milestones { get; set; }

        public Project()
        {
            Bids = new HashSet<Bid>();
            Milestones = new HashSet<Milestone>();
        }
    }
}
