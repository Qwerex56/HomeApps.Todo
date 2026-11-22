using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class EventSeriesConfiguration : IEntityTypeConfiguration<EventSeries>
{
    public void Configure(EntityTypeBuilder<EventSeries> builder)
    {
        builder.ToTable("EventSeries");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(s => s.Title).HasConversion(Converters.EventTitle).HasMaxLength(EventTitle.MaxLength);
        builder.Property(s => s.Description).HasMaxLength(4000);
        builder.Property(s => s.Location).HasMaxLength(500);
        builder.Property(s => s.TimeZoneId).HasMaxLength(64);
        builder.Property(s => s.Rule).HasColumnName("Rrule").HasConversion(Converters.RecurrenceRule).HasMaxLength(RecurrenceRule.MaxLength);

        builder.HasIndex(s => s.HouseholdId);
        builder.HasIndex(s => new { s.HouseholdId, s.OwnerLeftAt });
    }
}
