using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalHouse.Domain.Entities;

namespace RentalHouse.Infrastructure.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasOne(c => c.Tenant)
               .WithMany()
               .HasForeignKey(c => c.TenantId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Host)
               .WithMany()
               .HasForeignKey(c => c.HostId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}