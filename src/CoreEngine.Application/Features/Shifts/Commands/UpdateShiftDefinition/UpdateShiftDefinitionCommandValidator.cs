using FluentValidation;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftDefinition;

public class UpdateShiftDefinitionCommandValidator : AbstractValidator<UpdateShiftDefinitionCommand>
{
    public UpdateShiftDefinitionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Shift definition ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Shift definition name is required.")
            .MaximumLength(100).WithMessage("Shift definition name must not exceed 100 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(20).WithMessage("Code must not exceed 20 characters.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.")
            .Must(BeAValidTimeSpan).WithMessage("Start time must be a valid time format (HH:mm).");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .Must(BeAValidTimeSpan).WithMessage("End time must be a valid time format (HH:mm).");

        RuleFor(x => x.ShiftOrder)
            .InclusiveBetween(0, 10)
            .WithMessage("Shift order must be between 0 and 10.");

        RuleFor(x => x.Color)
            .MaximumLength(20).WithMessage("Color must not exceed 20 characters.");
    }

    private static bool BeAValidTimeSpan(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return TimeSpan.TryParse(value, out _);
    }
}
