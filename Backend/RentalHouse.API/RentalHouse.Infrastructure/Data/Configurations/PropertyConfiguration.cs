using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Address).IsRequired().HasMaxLength(500);
        builder.Property(p => p.Description).HasColumnType("text"); // Lưu text dài

        // Chuẩn hóa Decimal
        builder.Property(p => p.PricePerNight).IsRequired().HasColumnType("decimal(18,2)");

        // Restrict Delete (Ngắt lặp vòng)
        builder.HasOne(p => p.Host)
               .WithMany(u => u.Properties)
               .HasForeignKey(p => p.HostId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.HostId);
    }
}