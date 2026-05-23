using System;

namespace FreelanceHub.Domain.Entities
{
    public class ClientProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyWebsite { get; set; }
        public string Industry { get; set; }
        public string CompanySize { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int ProjectsPosted { get; set; }
        public decimal TotalSpent { get; set; }
        public string PaymentMethod { get; set; }
        public bool PaymentVerified { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
