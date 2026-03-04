using FluentValidation;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftInstance;

public class CreateShiftInstanceCommandValidator : AbstractValidator<CreateShiftInstanceCommand>
{
    private static readonly string[] ValidStatuses = { "Scheduled", "InProgress", "Completed", "Cancelled" };

    public CreateShiftInstanceCommandValidator()
    {
        RuleFor(x => x.ShiftDefinitionId)
            .NotEmpty().WithMessage("Shift definition ID is required.");

        RuleFor(x => x.MineSiteId)
            .NotEmpty().WithMessage("Mine site ID is required.");

        RuleFor(x => x.Date)
            .NotEmpty().WithMessage("Date is required.")
            .Must(BeAValidDate).WithMessage("Date must be a valid date format (yyyy-MM-dd).");

        RuleFor(x => x.SupervisorName)
            .MaximumLength(200).WithMessage("Supervisor name must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .Must(x => x == null || ValidStatuses.Contains(x))
            .WithMessage("Status must be Scheduled, InProgress, Completed, or Cancelled.");

        RuleFor(x => x.PersonnelCount)
            .GreaterThanOrEqualTo(0).When(x => x.PersonnelCount.HasValue)
            .WithMessage("Personnel count must be a non-negative number.");

        RuleFor(x => x.WeatherConditions)
            .MaximumLength(500).WithMessage("Weather conditions must not exceed 500 characters.");

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.");
    }

    private static bool BeAValidDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return DateOnly.TryParse(value, out _);
    }
}
