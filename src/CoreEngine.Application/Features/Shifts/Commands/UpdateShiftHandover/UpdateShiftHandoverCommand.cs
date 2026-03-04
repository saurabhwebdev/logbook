using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftHandover;

public record UpdateShiftHandoverCommand(
    Guid Id,
    Guid? IncomingShiftInstanceId,
    DateTime HandoverDateTime,
    string? SafetyIssues,
    string? OngoingOperations,
    string? PendingTasks,
    string? EquipmentStatus,
    string? EnvironmentalConditions,
    string? GeneralRemarks,
    string? HandedOverBy,
    string? ReceivedBy,
    string Status
) : IRequest<Unit>;
