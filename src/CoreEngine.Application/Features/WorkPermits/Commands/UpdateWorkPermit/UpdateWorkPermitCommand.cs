using MediatR;

namespace CoreEngine.Application.Features.WorkPermits.Commands.UpdateWorkPermit;

public record UpdateWorkPermitCommand(
    Guid Id,
    string Title,
    string PermitType,
    string RequestedBy,
    DateTime RequestDate,
    DateTime StartDateTime,
    DateTime EndDateTime,
    string Location,
    string WorkDescription,
    string? HazardsIdentified,
    string? ControlMeasures,
    string? PPERequired,
    string? EmergencyProcedures,
    bool GasTestRequired,
    string? GasTestResults,
    string Status,
    string? ApprovedBy,
    DateTime? ApprovedAt,
    string? ClosedBy,
    DateTime? ClosedAt,
    string? RejectionReason,
    string? Notes
) : IRequest;
