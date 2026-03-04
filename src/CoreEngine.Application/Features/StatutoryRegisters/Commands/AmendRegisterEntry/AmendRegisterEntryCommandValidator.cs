using FluentValidation;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.AmendRegisterEntry;

public class AmendRegisterEntryCommandValidator : AbstractValidator<AmendRegisterEntryCommand>
{
    private static readonly string[] ValidStatuses = { "Open", "ActionRequired", "Closed" };

    public AmendRegisterEntryCommandValidator()
    {
        RuleFor(x => x.OriginalEntryId)
            .NotEmpty().WithMessage("Original entry ID is required.");

        RuleFor(x => x.EntryDate)
            .NotEmpty().WithMessage("Entry date is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters.");

        RuleFor(x => x.Details)
            .NotEmpty().WithMessage("Details are required.")
            .MaximumLength(4000).WithMessage("Details must not exceed 4000 characters.");

        RuleFor(x => x.ReportedBy)
            .NotEmpty().WithMessage("Reported by is required.")
            .MaximumLength(200).WithMessage("Reported by must not exceed 200 characters.");

        RuleFor(x => x.WitnessName)
            .MaximumLength(200).WithMessage("Witness name must not exceed 200 characters.");

        RuleFor(x => x.ActionTaken)
            .MaximumLength(2000).WithMessage("Action taken must not exceed 2000 characters.");

        RuleFor(x => x.Status)
            .Must(x => x == null || ValidStatuses.Contains(x)).WithMessage("Invalid status. Must be Open, ActionRequired, or Closed.");

        RuleFor(x => x.AmendmentReason)
            .NotEmpty().WithMessage("Amendment reason is required.")
            .MaximumLength(1000).WithMessage("Amendment reason must not exceed 1000 characters.");
    }
}
