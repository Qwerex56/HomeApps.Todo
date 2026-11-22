using FluentValidation;

namespace Todo.Application.Events.CreateCalendarEvent;

internal sealed class CreateCalendarEventValidator : AbstractValidator<CreateCalendarEventCommand>
{
    public CreateCalendarEventValidator() => RuleFor(c => c.Event).NotNull().SetValidator(new CalendarEventInputValidator());
}
