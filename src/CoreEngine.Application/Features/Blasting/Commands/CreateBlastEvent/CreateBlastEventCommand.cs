using MediatR;

namespace CoreEngine.Application.Features.Blasting.Commands.CreateBlastEvent;

public record CreateBlastEventCommand(
    Guid MineSiteId,
    Guid? MineAreaId,
    string Title,
    string BlastType,
    DateTime ScheduledDateTime,
    string Location,
    string? DrillingPattern,
    int? NumberOfHoles,
    decimal? TotalExplosivesKg,
    string? ExplosiveType,
    string? DetonatorType,
    string? BlastDesignNotes,
    double? SafetyRadius,
    string SupervisorName,
    string LicensedBlasterName
) : IRequest<Guid>;
