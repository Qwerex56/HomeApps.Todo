using FluentValidation;

namespace Todo.Application.ExternalCalendars.RemoveExternalCalendar;

internal sealed class RemoveExternalCalendarValidator : AbstractValidator<RemoveExternalCalendarCommand>
{
    public RemoveExternalCalendarValidator() => RuleFor(c => c.Id).NotEmpty();
}
