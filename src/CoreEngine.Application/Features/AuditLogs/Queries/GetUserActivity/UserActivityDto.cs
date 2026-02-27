namespace CoreEngine.Application.Features.AuditLogs.Queries.GetUserActivity;

public record UserActivityDto(
    long Id,
    string Action,
    string EntityName,
    string EntityId,
    string? OldValues,
    string? NewValues,
    DateTime Timestamp
);
