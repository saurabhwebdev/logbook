namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class Notification : TenantScopedEntity
{
    public Guid RecipientUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Info, Success, Warning, Error
    public string? Link { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
