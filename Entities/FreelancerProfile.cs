using System;
using System.Collections.Generic;
using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Domain.Entities
{
    public class FreelancerProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Overview { get; set; }
        public decimal HourlyRate { get; set; }
        public string Currency { get; set; }
        public int YearsOfExperience { get; set; }
        public string PortfolioUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string GitHubUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int CompletedProjects { get; set; }
        public decimal TotalEarnings { get; set; }
        public FreelancerAvailability Availability { get; set; }
        public string Languages { get; set; }
        public string Skills { get; set; }
        public string Certifications { get; set; }
        public string Education { get; set; }

        // Navigation property
        public virtual User User { get; set; }
    }
}
