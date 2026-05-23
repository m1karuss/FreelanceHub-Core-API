using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.ProfileImageUrl)
                .HasMaxLength(500);

            builder.Property(u => u.Bio)
                .HasMaxLength(2000);

            builder.Property(u => u.Country)
                .HasMaxLength(100);

            builder.Property(u => u.City)
                .HasMaxLength(100);

            builder.Property(u => u.TimeZone)
                .HasMaxLength(100);

            builder.Property(u => u.ProfileCompletionPercentage)
                .HasColumnType("decimal(5,2)");

            builder.HasQueryFilter(u => !u.IsDeleted);

            builder.HasOne(u => u.FreelancerProfile)
                .WithOne(f => f.User)
                .HasForeignKey<FreelancerProfile>(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.ClientProfile)
                .WithOne(c => c.User)
                .HasForeignKey<ClientProfile>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.RefreshTokens)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.ProjectsCreated)
                .WithOne(p => p.Client)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Bids)
                .WithOne(b => b.Freelancer)
                .HasForeignKey(b => b.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.SentMessages)
                .WithOne(m => m.Sender)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.Receiver)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Notifications)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.Activities)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
