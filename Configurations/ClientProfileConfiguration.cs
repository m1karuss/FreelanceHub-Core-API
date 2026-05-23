using FreelanceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class ClientProfileConfiguration : IEntityTypeConfiguration<ClientProfile>
    {
        public void Configure(EntityTypeBuilder<ClientProfile> builder)
        {
            builder.ToTable("ClientProfiles");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CompanyName)
                .HasMaxLength(200);

            builder.Property(c => c.CompanyWebsite)
                .HasMaxLength(500);

            builder.Property(c => c.Industry)
                .HasMaxLength(100);

            builder.Property(c => c.CompanySize)
                .HasMaxLength(50);

            builder.Property(c => c.AverageRating)
                .HasColumnType("decimal(3,2)");

            builder.Property(c => c.TotalSpent)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.PaymentMethod)
                .HasMaxLength(100);

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.HasIndex(c => c.UserId)
                .IsUnique();
        }
    }
}
