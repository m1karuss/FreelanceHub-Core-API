using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class BidConfiguration : IEntityTypeConfiguration<Bid>
    {
        public void Configure(EntityTypeBuilder<Bid> builder)
        {
            builder.ToTable("Bids");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Currency)
                .HasMaxLength(10);

            builder.Property(b => b.DeliveryTimeUnit)
                .HasMaxLength(50);

            builder.Property(b => b.CoverLetter)
                .HasMaxLength(3000);

            builder.Property(b => b.RejectionReason)
                .HasMaxLength(1000);

            builder.HasQueryFilter(b => !b.IsDeleted);

            builder.HasIndex(b => new { b.ProjectId, b.FreelancerId })
                .IsUnique();

            builder.HasIndex(b => b.Status);

            builder.HasOne(b => b.Project)
                .WithMany(p => p.Bids)
                .HasForeignKey(b => b.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Freelancer)
                .WithMany(u => u.Bids)
                .HasForeignKey(b => b.FreelancerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
