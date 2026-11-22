using FluentValidation;
using Todo.Domain.Events;

namespace Todo.Application.Subscriptions.CreateSubscription;

internal sealed class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionValidator()
    {
        RuleFor(c => c.Label).NotEmpty().MaximumLength(CalendarSubscription.MaxLabelLength);
        RuleFor(c => c.TasksMode).IsInEnum();
    }
}
