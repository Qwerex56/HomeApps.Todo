using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Events;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class ExternalCalendarConfig : IEntityTypeConfiguration<ExternalCalendarImport>
{
    public void Configure(EntityTypeBuilder<ExternalCalendarImport> builder)
    {
        builder.ToTable("ExternalCalendarImports");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Url).HasMaxLength(ExternalCalendarImport.MaxUrlLength);
        builder.Property(c => c.Name).HasMaxLength(ExternalCalendarImport.MaxNameLength);
        builder.Property(c => c.Color).HasMaxLength(16);
        builder.Property(c => c.Visibility).HasConversion<string>().HasMaxLength(Converters.EnumLength);
        builder.Property(c => c.LastErrorMessage).HasMaxLength(500);
        builder.Ignore(c => c.IsPaused);

        builder.HasIndex(c => new { c.HouseholdId, c.OwnerLeftAt }).HasDatabaseName("ix_external_calendars_owner_left");
        builder.HasIndex(c => new { c.HouseholdId, c.UserId });
    }
}
