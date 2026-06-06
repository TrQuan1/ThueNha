using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.HasQueryFilter(f => !f.IsDeleted);

        builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
        builder.Property(f => f.IconUrl).HasMaxLength(500);
    }
}