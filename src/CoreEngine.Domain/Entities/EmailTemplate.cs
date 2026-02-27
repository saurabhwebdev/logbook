namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class EmailTemplate : TenantScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string HtmlBody { get; set; } = string.Empty;
    public string? PlainTextBody { get; set; }
    public bool IsActive { get; set; } = true;
}
