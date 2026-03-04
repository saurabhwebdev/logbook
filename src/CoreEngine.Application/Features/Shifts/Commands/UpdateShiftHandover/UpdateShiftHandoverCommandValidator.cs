using FluentValidation;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftHandover;

public class UpdateShiftHandoverCommandValidator : AbstractValidator<UpdateShiftHandoverCommand>
{
    private static readonly string[] ValidStatuses = { "Draft", "Submitted", "Acknowledged" };

    public UpdateShiftHandoverCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Handover ID is required.");

        RuleFor(x => x.HandoverDateTime)
            .NotEmpty().WithMessage("Handover date/time is required.");

        RuleFor(x => x.SafetyIssues)
            .MaximumLength(2000).WithMessage("Safety issues must not exceed 2000 characters.");

        RuleFor(x => x.OngoingOperations)
            .MaximumLength(2000).WithMessage("Ongoing operations must not exceed 2000 characters.");

        RuleFor(x => x.PendingTasks)
            .MaximumLength(2000).WithMessage("Pending tasks must not exceed 2000 characters.");

        RuleFor(x => x.EquipmentStatus)
            .MaximumLength(2000).WithMessage("Equipment status must not exceed 2000 characters.");

        RuleFor(x => x.EnvironmentalConditions)
            .MaximumLength(1000).WithMessage("Environmental conditions must not exceed 1000 characters.");

        RuleFor(x => x.GeneralRemarks)
            .MaximumLength(2000).WithMessage("General remarks must not exceed 2000 characters.");

        RuleFor(x => x.HandedOverBy)
            .MaximumLength(200).WithMessage("Handed over by must not exceed 200 characters.");

        RuleFor(x => x.ReceivedBy)
            .MaximumLength(200).WithMessage("Received by must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(x => ValidStatuses.Contains(x))
            .WithMessage("Status must be Draft, Submitted, or Acknowledged.");
    }
}
