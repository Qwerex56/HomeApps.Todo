using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>An event's title: 1–200 characters, trimmed, one line.</summary>
public sealed record EventTitle
{
    public const int MaxLength = 200;

    private EventTitle(string value) => Value = value;

    public string Value { get; }

    public static Result<EventTitle> Create(string? raw)
    {
        var value = raw?.Trim() ?? string.Empty;
        if (value.Length is 0 or > MaxLength || Text.HasControlCharacters(value))
        {
            return Result.Fail<EventTitle>(PlannerErrors.EmptyTitle);
        }

        return new EventTitle(value);
    }

    public override string ToString() => Value;
}
