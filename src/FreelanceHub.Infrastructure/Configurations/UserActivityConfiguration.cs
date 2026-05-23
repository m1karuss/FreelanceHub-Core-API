using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.ToTable("UserActivities");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.IpAddress)
                .HasMaxLength(50);

            builder.Property(a => a.UserAgent)
                .HasMaxLength(500);

            builder.Property(a => a.Metadata)
                .HasMaxLength(2000);

            builder.HasQueryFilter(a => !a.IsDeleted);

            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.Type);
            builder.HasIndex(a => a.CreatedAt);

            builder.HasOne(a => a.User)
                .WithMany(u => u.Activities)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
