using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.Property(b => b.TotalPrice).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasOne(b => b.Tenant)
               .WithMany(u => u.Bookings)
               .HasForeignKey(b => b.TenantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Property)
               .WithMany(p => p.Bookings)
               .HasForeignKey(b => b.PropertyId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => b.PropertyId);
    }
}