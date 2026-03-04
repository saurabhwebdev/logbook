using MediatR;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftHandover;

public record CreateShiftHandoverCommand(
    Guid OutgoingShiftInstanceId,
    Guid? IncomingShiftInstanceId,
    Guid MineSiteId,
    DateTime HandoverDateTime,
    string? SafetyIssues,
    string? OngoingOperations,
    string? PendingTasks,
    string? EquipmentStatus,
    string? EnvironmentalConditions,
    string? GeneralRemarks,
    string? HandedOverBy,
    string? ReceivedBy,
    string? Status
) : IRequest<Guid>;
