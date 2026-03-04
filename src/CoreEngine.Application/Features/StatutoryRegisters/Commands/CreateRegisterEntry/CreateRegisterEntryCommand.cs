using MediatR;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateRegisterEntry;

public record CreateRegisterEntryCommand(
    Guid StatutoryRegisterId,
    Guid MineSiteId,
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
    string? Status
) : IRequest<Guid>;
