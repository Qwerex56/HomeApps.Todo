using FluentValidation;

namespace Todo.Application.Events.GetCalendarItems;

internal sealed class GetCalendarItemsValidator : AbstractValidator<GetCalendarItemsQuery>
{
    public static readonly string[] KnownLayers = ["events", "private", "tasks", "external"];

    public GetCalendarItemsValidator()
    {
        RuleFor(q => q.To).GreaterThan(q => q.From).WithMessage("'to' must be after 'from'.");
        RuleFor(q => q).Must(q => q.To - q.From <= TimeSpan.FromDays(GetCalendarItemsQuery.MaxDays))
            .WithMessage($"The window is at most {GetCalendarItemsQuery.MaxDays} days.");
        RuleFor(q => q.Layers).Must(l => string.IsNullOrWhiteSpace(l) || Split(l).All(x => KnownLayers.Contains(x)))
            .WithMessage("Layers are events, private, tasks and external.");
    }

    public static string[] Split(string layers) =>
        layers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x => x.ToLowerInvariant()).ToArray();
}
