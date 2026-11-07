using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Lists;

namespace Todo.Infrastructure.Persistence.Configurations;

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
