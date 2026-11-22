using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class EventReminderOverrideConfig : IEntityTypeConfiguration<EventReminderOverride>
{
    public void Configure(EntityTypeBuilder<EventReminderOverride> builder)
    {
        builder.ToTable("EventReminderOverrides");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();
        builder.Property(o => o.Reminders).HasConversion(Converters.Reminders, Converters.RemindersComparer).HasColumnType("jsonb");

        builder.HasOne<CalendarEvent>().WithMany().HasForeignKey(o => o.EventId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(o => new { o.EventId, o.UserId }).IsUnique().HasDatabaseName("ux_event_reminder_overrides_event_user");
    }
}
