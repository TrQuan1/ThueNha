using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.HasQueryFilter(pi => !pi.IsDeleted);

        builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(1000);
        builder.Property(pi => pi.IsThumbnail).HasDefaultValue(false);

        builder.HasOne(pi => pi.Property)
               .WithMany(p => p.Images)
               .HasForeignKey(pi => pi.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}