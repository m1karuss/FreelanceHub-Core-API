using System;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public string ActionUrl { get; set; }
        public string Metadata { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
