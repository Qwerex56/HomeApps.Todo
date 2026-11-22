using FluentValidation;

namespace Todo.Application.Settings.UpdatePlannerSettings;

internal sealed class UpdatePlannerSettingsValidator : AbstractValidator<UpdatePlannerSettingsCommand>
{
    public UpdatePlannerSettingsValidator()
    {
        RuleFor(c => c.FirstDayOfWeek).IsInEnum();
        RuleFor(c => c.TimeZone).MaximumLength(100);
    }
}
