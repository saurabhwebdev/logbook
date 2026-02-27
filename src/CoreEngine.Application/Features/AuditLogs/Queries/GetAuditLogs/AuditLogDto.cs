namespace CoreEngine.Application.Features.AuditLogs.Queries.GetAuditLogs;

public record AuditLogDto(
    long Id,
    string? UserId,
    string Action,
    string EntityName,
    string EntityId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    DateTime Timestamp
);
