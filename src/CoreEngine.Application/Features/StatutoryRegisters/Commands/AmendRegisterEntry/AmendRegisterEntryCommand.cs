using MediatR;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.AmendRegisterEntry;

public record AmendRegisterEntryCommand(
    Guid OriginalEntryId,
    DateTime EntryDate,
    Guid? ShiftInstanceId,
    Guid? MineAreaId,
    string Subject,
    string Details,
    string ReportedBy,
    string? WitnessName,
    string? ActionTaken,
    DateTime? ActionDueDate,
    DateTime? ActionCompletedDate,
    string? Status,
    string AmendmentReason
) : IRequest<Guid>;
