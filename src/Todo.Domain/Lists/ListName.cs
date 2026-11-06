using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Lists;

/// <summary>A list's name: 1–40 characters, trimmed, no control characters.</summary>
public sealed record ListName
{
    public const int MaxLength = 40;

    private ListName(string value) => Value = value;

    public string Value { get; }

    public static Result<ListName> Create(string? raw)
    {
        var value = raw?.Trim() ?? string.Empty;
        if (value.Length is 0 or > MaxLength || Text.HasControlCharacters(value))
        {
            return Result.Fail<ListName>(TodoErrors.ListNameInvalid);
        }

        return new ListName(value);
    }

    public override string ToString() => Value;
}
