using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Households;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class InactiveMarkerConfig : IEntityTypeConfiguration<InactiveMarker>
{
    public void Configure(EntityTypeBuilder<InactiveMarker> builder)
    {
        builder.ToTable("InactiveMarkers");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();
        builder.HasIndex(m => new { m.HouseholdId, m.UserId });
    }
}
