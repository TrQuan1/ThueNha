using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class PropertyFacilityConfiguration : IEntityTypeConfiguration<PropertyFacility>
{
    public void Configure(EntityTypeBuilder<PropertyFacility> builder)
    {
        // Khóa chính tổ hợp
        builder.HasKey(pf => new { pf.PropertyId, pf.FacilityId });
    }
}