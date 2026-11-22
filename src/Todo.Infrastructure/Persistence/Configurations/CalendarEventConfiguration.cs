using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("CalendarEvents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(e => e.Title).HasConversion(Converters.EventTitle).HasMaxLength(EventTitle.MaxLength);
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.Location).HasMaxLength(500);
        builder.Property(e => e.TimeZoneId).HasMaxLength(64);
        builder.Property(e => e.Attendees).HasColumnType("jsonb");
        builder.Property(e => e.Reminders).HasConversion(Converters.Reminders, Converters.RemindersComparer).HasColumnType("jsonb");
        builder.Ignore(e => e.IsForWholeHousehold);

        builder.HasOne<EventSeries>().WithMany().HasForeignKey(e => e.SeriesId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(e => new { e.HouseholdId, e.StartUtc, e.EndUtc }).HasDatabaseName("ix_calendar_events_household_start_end");
        builder.HasIndex(e => new { e.CreatedBy, e.IsPrivate }).HasDatabaseName("ix_calendar_events_created_by_private");
        builder.HasIndex(e => new { e.HouseholdId, e.OwnerLeftAt }).HasDatabaseName("ix_calendar_events_owner_left");
    }
}
