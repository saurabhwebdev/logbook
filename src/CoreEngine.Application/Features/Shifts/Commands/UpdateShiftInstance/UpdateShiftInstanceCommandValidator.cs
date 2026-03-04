using FluentValidation;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftInstance;

public class UpdateShiftInstanceCommandValidator : AbstractValidator<UpdateShiftInstanceCommand>
{
    private static readonly string[] ValidStatuses = { "Scheduled", "InProgress", "Completed", "Cancelled" };

    public UpdateShiftInstanceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Shift instance ID is required.");

        RuleFor(x => x.SupervisorName)
            .MaximumLength(200).WithMessage("Supervisor name must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(x => ValidStatuses.Contains(x))
            .WithMessage("Status must be Scheduled, InProgress, Completed, or Cancelled.");

        RuleFor(x => x.PersonnelCount)
            .GreaterThanOrEqualTo(0).When(x => x.PersonnelCount.HasValue)
            .WithMessage("Personnel count must be a non-negative number.");

        RuleFor(x => x.WeatherConditions)
            .MaximumLength(500).WithMessage("Weather conditions must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");
    }
}
