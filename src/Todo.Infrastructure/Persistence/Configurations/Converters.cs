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
