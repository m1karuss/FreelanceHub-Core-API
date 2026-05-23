using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class FreelancerProfileConfiguration : IEntityTypeConfiguration<FreelancerProfile>
    {
        public void Configure(EntityTypeBuilder<FreelancerProfile> builder)
        {
            builder.ToTable("FreelancerProfiles");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Title)
                .HasMaxLength(200);

            builder.Property(f => f.Overview)
                .HasMaxLength(3000);

            builder.Property(f => f.HourlyRate)
                .HasColumnType("decimal(18,2)");

            builder.Property(f => f.Currency)
                .HasMaxLength(10);

            builder.Property(f => f.PortfolioUrl)
                .HasMaxLength(500);

            builder.Property(f => f.LinkedInUrl)
                .HasMaxLength(500);

            builder.Property(f => f.GitHubUrl)
                .HasMaxLength(500);

            builder.Property(f => f.AverageRating)
                .HasColumnType("decimal(3,2)");

            builder.Property(f => f.TotalEarnings)
                .HasColumnType("decimal(18,2)");

            builder.Property(f => f.Languages)
                .HasMaxLength(500);

            builder.Property(f => f.Skills)
                .HasMaxLength(2000);

            builder.Property(f => f.Certifications)
                .HasMaxLength(2000);

            builder.Property(f => f.Education)
                .HasMaxLength(2000);

            builder.HasQueryFilter(f => !f.IsDeleted);

            builder.HasIndex(f => f.UserId)
                .IsUnique();
        }
    }
}
