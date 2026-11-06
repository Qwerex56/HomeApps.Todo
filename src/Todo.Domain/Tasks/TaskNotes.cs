using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Tasks;

/// <summary>Free notes under a task: at most 2000 characters, new lines allowed. Blank notes are no notes.</summary>
public sealed record TaskNotes
{
    public const int MaxLength = 2000;

    private TaskNotes(string value) => Value = value;

    public string Value { get; }

    /// <summary>Null when <paramref name="raw"/> is blank.</summary>
    public static Result<TaskNotes?> Create(string? raw)
    {
        var value = Text.OrNull(raw);
        if (value is null)
        {
            return Result.Ok<TaskNotes?>(null);
        }

        if (value.Length > MaxLength || Text.HasControlCharacters(value, multiline: true))
        {
            return Result.Fail<TaskNotes?>(TodoErrors.TaskNotesInvalid);
        }

        return new TaskNotes(value);
    }

    public override string ToString() => Value;
}
