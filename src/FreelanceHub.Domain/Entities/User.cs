using System;
using System.Collections.Generic;
using FreelanceHub.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FreelanceHub.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Bio { get; set; }
        public UserRole Role { get; set; }
        public UserStatus Status { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? EmailVerifiedAt { get; set; }
        public string EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockedUntil { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string TimeZone { get; set; }
        public decimal ProfileCompletionPercentage { get; set; }

        // BaseEntity properties
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual FreelancerProfile FreelancerProfile { get; set; }
        public virtual ClientProfile ClientProfile { get; set; }
        public virtual ICollection<Project> ProjectsCreated { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; }
        public virtual ICollection<Message> ReceivedMessages { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Payment> PaymentsSent { get; set; }
        public virtual ICollection<Payment> PaymentsReceived { get; set; }
        public virtual ICollection<Review> ReviewsGiven { get; set; }
        public virtual ICollection<Review> ReviewsReceived { get; set; }
        public virtual ICollection<UserActivity> Activities { get; set; }

        public User()
        {
            RefreshTokens = new HashSet<RefreshToken>();
            ProjectsCreated = new HashSet<Project>();
            Bids = new HashSet<Bid>();
            SentMessages = new HashSet<Message>();
            ReceivedMessages = new HashSet<Message>();
            Notifications = new HashSet<Notification>();
            PaymentsSent = new HashSet<Payment>();
            PaymentsReceived = new HashSet<Payment>();
            ReviewsGiven = new HashSet<Review>();
            ReviewsReceived = new HashSet<Review>();
            Activities = new HashSet<UserActivity>();
        }
    }
}
