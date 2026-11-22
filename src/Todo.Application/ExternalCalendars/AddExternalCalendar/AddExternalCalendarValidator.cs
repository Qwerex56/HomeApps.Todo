using FluentValidation;
using Todo.Domain.Events;

namespace Todo.Application.ExternalCalendars.AddExternalCalendar;

internal sealed class AddExternalCalendarValidator : AbstractValidator<AddExternalCalendarCommand>
{
    public AddExternalCalendarValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(ExternalCalendarImport.MaxNameLength);
        RuleFor(c => c.Color).MaximumLength(ExternalCalendarImport.MaxColorLength);
        RuleFor(c => c.Visibility).IsInEnum();
    }
}
