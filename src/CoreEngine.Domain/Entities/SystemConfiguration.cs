namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class SystemConfiguration : TenantScopedEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public string? Description { get; set; }
    public string DataType { get; set; } = "String"; // String, Int, Bool, Json
}
