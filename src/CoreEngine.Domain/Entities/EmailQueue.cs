namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;
using CoreEngine.Domain.Enums;

public class EmailQueue : TenantScopedEntity
{
    public string To { get; set; } = string.Empty;
    public string? Cc { get; set; }
    public string? Bcc { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? PlainTextBody { get; set; }
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    public DateTime? SentAt { get; set; }
    public string? FailureReason { get; set; }
    public int RetryCount { get; set; } = 0;
}
