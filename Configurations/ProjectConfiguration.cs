using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(p => p.Category)
                .HasMaxLength(100);

            builder.Property(p => p.Skills)
                .HasMaxLength(1000);

            builder.Property(p => p.Budget)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                .HasMaxLength(10);

            builder.Property(p => p.DurationUnit)
                .HasMaxLength(50);

            builder.Property(p => p.Attachments)
                .HasMaxLength(2000);

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.Category);
            builder.HasIndex(p => p.PublishedAt);

            builder.HasOne(p => p.Client)
                .WithMany(u => u.ProjectsCreated)
                .HasForeignKey(p => p.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.AwardedFreelancer)
                .WithMany()
                .HasForeignKey(p => p.AwardedFreelancerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Bids)
                .WithOne(b => b.Project)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Milestones)
                .WithOne(m => m.Project)
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
