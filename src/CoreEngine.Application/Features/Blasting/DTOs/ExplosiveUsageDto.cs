namespace CoreEngine.Application.Features.Blasting.DTOs;

public record ExplosiveUsageDto(
    Guid Id,
    Guid BlastEventId,
    string BlastTitle,
    string ExplosiveName,
    string Type,
    string? BatchNumber,
    decimal QuantityIssued,
    decimal QuantityUsed,
    decimal QuantityReturned,
    string Unit,
    string? MagazineSource,
    string? IssuedBy,
    string? ReceivedBy,
    string? Notes,
    DateTime CreatedAt
);
