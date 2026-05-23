using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            builder.ToTable("Milestones");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(2000);

            builder.Property(m => m.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(m => m.Currency)
                .HasMaxLength(10);

            builder.HasQueryFilter(m => !m.IsDeleted);

            builder.HasIndex(m => m.ProjectId);
            builder.HasIndex(m => m.Status);
        }
    }
}
