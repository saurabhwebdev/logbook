namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class HelpArticle : TenantScopedEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ModuleKey { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; } = true;
    public string? Tags { get; set; }
}
