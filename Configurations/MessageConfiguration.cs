using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Subject)
                .HasMaxLength(200);

            builder.Property(m => m.Body)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(m => m.Attachments)
                .HasMaxLength(2000);

            builder.HasQueryFilter(m => !m.IsDeleted);

            builder.HasIndex(m => m.SenderId);
            builder.HasIndex(m => m.ReceiverId);
            builder.HasIndex(m => m.IsRead);
            builder.HasIndex(m => m.CreatedAt);

            builder.HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.Project)
                .WithMany()
                .HasForeignKey(m => m.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
