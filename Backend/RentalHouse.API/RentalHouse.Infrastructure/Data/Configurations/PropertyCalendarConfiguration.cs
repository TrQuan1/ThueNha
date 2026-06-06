using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class PropertyCalendarConfiguration : IEntityTypeConfiguration<PropertyCalendar>
{
    public void Configure(EntityTypeBuilder<PropertyCalendar> builder)
    {
        builder.HasQueryFilter(pc => !pc.IsDeleted);

        builder.Property(pc => pc.PriceOverride).HasColumnType("decimal(18,2)");

        builder.HasOne(pc => pc.Property)
               .WithMany(p => p.Calendars)
               .HasForeignKey(pc => pc.PropertyId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}