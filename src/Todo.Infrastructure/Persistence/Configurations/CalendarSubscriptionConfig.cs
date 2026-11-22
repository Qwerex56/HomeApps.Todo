using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class CalendarSubscriptionConfig : IEntityTypeConfiguration<CalendarSubscription>
{
    public void Configure(EntityTypeBuilder<CalendarSubscription> builder)
    {
        builder.ToTable("CalendarSubscriptions");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.TokenHash).HasMaxLength(64);
        builder.Property(s => s.Label).HasMaxLength(CalendarSubscription.MaxLabelLength);
        builder.Property(s => s.TasksMode).HasConversion<string>().HasMaxLength(Converters.EnumLength);
        builder.Ignore(s => s.Serves);

        builder.HasIndex(s => s.TokenHash).IsUnique().HasDatabaseName("ux_calendar_subscriptions_token_hash");
        builder.HasIndex(s => new { s.HouseholdId, s.UserId });
    }
}
