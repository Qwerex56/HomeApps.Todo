using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Tasks;

/// <summary>A task's title: 1–200 characters, trimmed, one line.</summary>
public sealed record TaskTitle
{
    public const int MaxLength = 200;

    private TaskTitle(string value) => Value = value;

    public string Value { get; }

    public static Result<TaskTitle> Create(string? raw)
    {
        var value = raw?.Trim() ?? string.Empty;
        if (value.Length is 0 or > MaxLength || Text.HasControlCharacters(value))
        {
            return Result.Fail<TaskTitle>(TodoErrors.TaskTitleInvalid);
        }

        return new TaskTitle(value);
    }

    public override string ToString() => Value;
}
