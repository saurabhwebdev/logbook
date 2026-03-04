using MediatR;

namespace CoreEngine.Application.Features.WorkPermits.Commands.CreateWorkPermit;

public record CreateWorkPermitCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
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
    string? Notes
) : IRequest<Guid>;
