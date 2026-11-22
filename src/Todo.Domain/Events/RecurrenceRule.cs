using FluentResults;
using Todo.Domain.Common;

namespace Todo.Domain.Events;

/// <summary>An RFC 5545 RRULE value such as <c>FREQ=WEEKLY;BYDAY=TU,TH</c>. Checked against what <see cref="RecurrenceEvaluator"/> supports.</summary>
public sealed record RecurrenceRule
{
    public const int MaxLength = 500;

    private RecurrenceRule(string value) => Value = value;

    public string Value { get; }

    public static Result<RecurrenceRule> Create(string? raw)
    {
        var value = raw?.Trim().ToUpperInvariant() ?? string.Empty;
        if (value.StartsWith("RRULE:", StringComparison.Ordinal))
        {
            value = value["RRULE:".Length..];
        }

        if (value.Length > MaxLength || !HasValidFrequency(value) || !RecurrenceEvaluator.IsSupported(value)
            || value.Any(c => !(char.IsAsciiLetterOrDigit(c) || c is '=' or ';' or ',' or '-' or '+')))
        {
            return Result.Fail<RecurrenceRule>(PlannerErrors.InvalidRule);
        }

        return new RecurrenceRule(value);
    }

    private static bool HasValidFrequency(string value) =>
        value.Split(';')[0] is "FREQ=DAILY" or "FREQ=WEEKLY" or "FREQ=MONTHLY" or "FREQ=YEARLY";

    public override string ToString() => Value;
}
