using FluentValidation;

namespace Todo.Application.Events.UpdateCalendarEvent;

internal sealed class UpdateCalendarEventValidator : AbstractValidator<UpdateCalendarEventCommand>
{
    public UpdateCalendarEventValidator() => RuleFor(c => c.Event).NotNull().SetValidator(new CalendarEventInputValidator());
}
