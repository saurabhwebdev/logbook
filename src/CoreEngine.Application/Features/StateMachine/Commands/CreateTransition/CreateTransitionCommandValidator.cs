using FluentValidation;

namespace CoreEngine.Application.Features.StateMachine.Commands.CreateTransition;

public class CreateTransitionCommandValidator : AbstractValidator<CreateTransitionCommand>
{
    public CreateTransitionCommandValidator()
    {
        RuleFor(x => x.EntityType)
            .NotEmpty().WithMessage("Entity type is required.")
            .MaximumLength(100).WithMessage("Entity type must not exceed 100 characters.");

        RuleFor(x => x.FromState)
            .NotEmpty().WithMessage("From state is required.")
            .MaximumLength(100).WithMessage("From state must not exceed 100 characters.");

        RuleFor(x => x.ToState)
            .NotEmpty().WithMessage("To state is required.")
            .MaximumLength(100).WithMessage("To state must not exceed 100 characters.");

        RuleFor(x => x.TriggerName)
            .NotEmpty().WithMessage("Trigger name is required.")
            .MaximumLength(100).WithMessage("Trigger name must not exceed 100 characters.")
            .Matches("^[A-Za-z][A-Za-z0-9]*$")
            .WithMessage("Trigger name must start with a letter and contain only alphanumeric characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.RequiredPermission)
            .MaximumLength(200).WithMessage("Required permission must not exceed 200 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.RequiredPermission));

        RuleFor(x => x)
            .Must(x => x.FromState != x.ToState)
            .WithMessage("From state and To state cannot be the same.");
    }
}
