using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence.Configurations;

/// <summary>Value objects stored as one column. Reading back goes through the domain factory, so bad data fails loudly.</summary>
internal static class Converters
{
    public static readonly ValueConverter<ListName, string> ListName = new(n => n.Value, v => Domain.Lists.ListName.Create(v).Value);

    public static readonly ValueConverter<TaskTitle, string> TaskTitle = new(t => t.Value, v => Domain.Tasks.TaskTitle.Create(v).Value);

    /// <summary>For nullable columns; EF never passes null to a converter, it stores null as is.</summary>
    public static readonly ValueConverter<TaskNotes?, string> TaskNotes = new(n => n!.Value, v => Domain.Tasks.TaskNotes.Create(v).Value!);

    /// <summary>Enums are stored as their names: readable in the database and safe if members are reordered.</summary>
    public const int EnumLength = 16;
}

internal sealed class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.ToTable("Lists");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();
        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(l => l.Name).HasConversion(Converters.ListName).HasMaxLength(ListName.MaxLength);
        builder.Property(l => l.Visibility).HasConversion<string>().HasMaxLength(Converters.EnumLength);
        builder.Ignore(l => l.IsArchived);
        builder.Ignore(l => l.IsShared);

        builder.HasIndex(l => new { l.HouseholdId, l.OwnerId });
        builder.HasIndex(l => l.OwnerLeftAt).HasFilter("\"OwnerLeftAt\" IS NOT NULL");
    }
}

internal sealed class TodoTaskConfiguration : IEntityTypeConfiguration<TodoTask>
{
    public void Configure(EntityTypeBuilder<TodoTask> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();
        builder.Property<uint>("Version").IsRowVersion();

        builder.Property(t => t.Title).HasConversion(Converters.TaskTitle).HasMaxLength(TaskTitle.MaxLength);
        builder.Property(t => t.Notes).HasConversion(Converters.TaskNotes).HasMaxLength(TaskNotes.MaxLength);
        builder.OwnsOne(t => t.Due, due =>
        {
            due.Property(d => d.Date).HasColumnName("DueDate");
            due.Property(d => d.Time).HasColumnName("DueTime");
        });
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(Converters.EnumLength);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(Converters.EnumLength);
        builder.Property(t => t.Position).HasColumnType("numeric");
        builder.Ignore(t => t.DomainEvents);

        builder.HasOne<TodoList>().WithMany().HasForeignKey(t => t.ListId).OnDelete(DeleteBehavior.Cascade);
        builder.HasIndex(t => new { t.ListId, t.Position });
        builder.HasIndex(t => new { t.HouseholdId, t.AssigneeId });
    }
}
