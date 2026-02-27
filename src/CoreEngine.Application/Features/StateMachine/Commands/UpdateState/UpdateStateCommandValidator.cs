using FluentValidation;

namespace CoreEngine.Application.Features.StateMachine.Commands.UpdateState;

public class UpdateStateCommandValidator : AbstractValidator<UpdateStateCommand>
{
    public UpdateStateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("State ID is required.");

        RuleFor(x => x.StateName)
            .NotEmpty().WithMessage("State name is required.")
            .MaximumLength(100).WithMessage("State name must not exceed 100 characters.")
            .Matches("^[A-Za-z][A-Za-z0-9]*$")
            .WithMessage("State name must start with a letter and contain only alphanumeric characters.");

        RuleFor(x => x.Color)
            .MaximumLength(50).WithMessage("Color must not exceed 50 characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.Color));

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort order must be a non-negative integer.");
    }
}
