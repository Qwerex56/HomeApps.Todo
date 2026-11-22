using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Settings;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class PlannerSettingsConfiguration : IEntityTypeConfiguration<PlannerSettings>
{
    public void Configure(EntityTypeBuilder<PlannerSettings> builder)
    {
        builder.ToTable("PlannerSettings");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();
        builder.Property(s => s.DefaultEventReminders).HasConversion(Converters.Reminders, Converters.RemindersComparer).HasColumnType("jsonb");
        builder.Property(s => s.DefaultTaskReminders).HasConversion(Converters.Reminders, Converters.RemindersComparer).HasColumnType("jsonb");
        builder.Property(s => s.FirstDayOfWeek).HasConversion<string>().HasMaxLength(Converters.EnumLength);
        builder.Property(s => s.TimeZoneId).HasMaxLength(100);

        builder.HasIndex(s => new { s.HouseholdId, s.UserId }).IsUnique().HasDatabaseName("ux_planner_settings_household_user");
    }
}
