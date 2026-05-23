using System;

namespace FreelanceHub.Domain.Entities
{
    public class Review : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid ReviewerId { get; set; }
        public Guid RevieweeId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Pros { get; set; }
        public string Cons { get; set; }
        public bool IsPublic { get; set; }
        public DateTime? PublishedAt { get; set; }

        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual User Reviewer { get; set; }
        public virtual User Reviewee { get; set; }
    }
}
