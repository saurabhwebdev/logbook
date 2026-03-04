using FluentValidation;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateStatutoryRegister;

public class CreateStatutoryRegisterCommandValidator : AbstractValidator<CreateStatutoryRegisterCommand>
{
    private static readonly string[] ValidRegisterTypes = { "Accident", "DangerousOccurrence", "PersonEntry", "Explosives", "MachineBreakdown", "Inspection", "WorkmenPresence", "Ventilation", "TimberSupply", "Custom" };

    public CreateStatutoryRegisterCommandValidator()
    {
        RuleFor(x => x.MineSiteId)
            .NotEmpty().WithMessage("Mine site is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Register name is required.")
            .MaximumLength(200).WithMessage("Register name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters.");

        RuleFor(x => x.RegisterType)
            .NotEmpty().WithMessage("Register type is required.")
            .Must(x => ValidRegisterTypes.Contains(x)).WithMessage("Invalid register type. Must be Accident, DangerousOccurrence, PersonEntry, Explosives, MachineBreakdown, Inspection, WorkmenPresence, Ventilation, TimberSupply, or Custom.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.Jurisdiction)
            .NotEmpty().WithMessage("Jurisdiction is required.")
            .MaximumLength(100).WithMessage("Jurisdiction must not exceed 100 characters.");

        RuleFor(x => x.RetentionYears)
            .InclusiveBetween(1, 100).When(x => x.RetentionYears.HasValue)
            .WithMessage("Retention years must be between 1 and 100.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).When(x => x.SortOrder.HasValue)
            .WithMessage("Sort order must be zero or greater.");
    }
}
