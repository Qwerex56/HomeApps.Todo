using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class EventOverrideConfiguration : IEntityTypeConfiguration<EventOverride>
{
    public void Configure(EntityTypeBuilder<EventOverride> builder)
    {
        builder.ToTable("EventOverrides");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();

        builder.Property(o => o.OverrideTitle).HasConversion(Converters.EventTitleOrNull).HasMaxLength(EventTitle.MaxLength);
        builder.Property(o => o.OverrideReminders).HasConversion(Converters.RemindersOrNull, Converters.RemindersOrNullComparer).HasColumnType("jsonb");

        builder.HasOne<EventSeries>().WithMany().HasForeignKey(o => o.SeriesId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(o => new { o.SeriesId, o.OriginalInstanceDate }).IsUnique().HasDatabaseName("ux_event_overrides_series_date");
    }
}
