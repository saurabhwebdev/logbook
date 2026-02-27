namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class FileMetadata : TenantScopedEntity
{
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string StoragePath { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public Guid UploadedByUserId { get; set; }
    public string? UploadedByName { get; set; }
}
