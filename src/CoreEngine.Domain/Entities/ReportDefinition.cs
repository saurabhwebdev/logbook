namespace CoreEngine.Domain.Entities;

using CoreEngine.Domain.Common;

public class ReportDefinition : TenantScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EntityType { get; set; } = string.Empty; // Which entity to report on
    public string ColumnsJson { get; set; } = "[]"; // JSON array of column definitions
    public string? FiltersJson { get; set; } // JSON object of default filters
    public string ExportFormat { get; set; } = "Excel"; // Excel, Csv
    public bool IsActive { get; set; } = true;
}
