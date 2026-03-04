using MediatR;

namespace CoreEngine.Application.Features.Blasting.Commands.UpdateBlastEvent;

public record UpdateBlastEventCommand(
    Guid Id,
    string Title,
    string BlastType,
    DateTime ScheduledDateTime,
    DateTime? ActualDateTime,
    string Location,
    string? DrillingPattern,
    int? NumberOfHoles,
    decimal? TotalExplosivesKg,
    string? ExplosiveType,
    string? DetonatorType,
    string Status,
    string? BlastDesignNotes,
    double? SafetyRadius,
    bool EvacuationConfirmed,
    bool SentryPostsConfirmed,
    bool PreBlastWarningGiven,
    string SupervisorName,
    string LicensedBlasterName,
    double? VibrationReading,
    double? AirBlastReading,
    string? PostBlastInspection,
    string? PostBlastNotes,
    string? FragmentationQuality,
    int MisfireCount
) : IRequest;
