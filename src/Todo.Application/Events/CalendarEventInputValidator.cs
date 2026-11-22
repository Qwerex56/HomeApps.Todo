using FluentValidation;

namespace Todo.Application.Events;

internal sealed class CalendarEventInputValidator : AbstractValidator<CalendarEventInput>
{
    public CalendarEventInputValidator()
    {
        RuleFor(e => e.Description).MaximumLength(4000);
        RuleFor(e => e.Location).MaximumLength(500);
        RuleFor(e => e.Attendees).Must(a => a is null || a.Count <= 50).WithMessage("An event has at most 50 attendees.");
        RuleFor(e => e.ReminderMinutes).Must(r => r is null || r.Count <= 10).WithMessage("An event has at most 10 reminders.");
        RuleFor(e => e).Must(e => e.Attendees is not { Count: > 0 } && e.ReminderMinutes is not { Count: > 0 })
            .When(e => !string.IsNullOrWhiteSpace(e.Rrule))
            .WithMessage("A recurring event has no attendees or reminders of its own.");
    }
}
