using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Amount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(p => p.PaymentMethod).HasMaxLength(50);
        builder.Property(p => p.TransactionNo).HasMaxLength(100);

        builder.HasOne(p => p.Booking)
               .WithOne(b => b.Payment)
               .HasForeignKey<Payment>(p => p.BookingId)
               .OnDelete(DeleteBehavior.Cascade); // Booking bị xóa vật lý thì Payment mới bay theo
    }
}