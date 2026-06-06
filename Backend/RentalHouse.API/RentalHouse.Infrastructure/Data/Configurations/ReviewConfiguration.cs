using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Property(r => r.Comment).HasMaxLength(1000);
        // Default value cho Rating nếu người dùng không truyền
        builder.Property(r => r.Rating).HasDefaultValue(5);

        builder.HasOne(r => r.Tenant)
               .WithMany(u => u.Reviews)
               .HasForeignKey(r => r.TenantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Booking)
               .WithOne(b => b.Review)
               .HasForeignKey<Review>(r => r.BookingId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}