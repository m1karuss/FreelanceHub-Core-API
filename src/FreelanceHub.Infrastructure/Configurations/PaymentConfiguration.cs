using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Currency)
                .HasMaxLength(10);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(200);

            builder.HasIndex(p => p.TransactionId)
                .IsUnique();

            builder.Property(p => p.PaymentMethod)
                .HasMaxLength(100);

            builder.Property(p => p.PlatformFee)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.NetAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.ProcessorResponse)
                .HasMaxLength(2000);

            builder.Property(p => p.Description)
                .HasMaxLength(1000);

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.ProcessedAt);

            builder.HasOne(p => p.Sender)
                .WithMany(u => u.PaymentsSent)
                .HasForeignKey(p => p.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Receiver)
                .WithMany(u => u.PaymentsReceived)
                .HasForeignKey(p => p.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Project)
                .WithMany()
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Milestone)
                .WithMany()
                .HasForeignKey(p => p.MilestoneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
