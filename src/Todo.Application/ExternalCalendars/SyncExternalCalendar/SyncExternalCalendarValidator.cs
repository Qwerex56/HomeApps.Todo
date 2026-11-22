using FluentValidation;

namespace Todo.Application.ExternalCalendars.SyncExternalCalendar;

internal sealed class SyncExternalCalendarValidator : AbstractValidator<SyncExternalCalendarCommand>
{
    public SyncExternalCalendarValidator() => RuleFor(c => c.Id).NotEmpty();
}
