using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Infrastructure.Persistence.Configurations;

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
