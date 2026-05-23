using System;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class UserActivity : BaseEntity
    {
        public Guid UserId { get; set; }
        public ActivityType Type { get; set; }
        public string Description { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Metadata { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
