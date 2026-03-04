using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class ShiftInstance : TenantScopedEntity
{
    public Guid ShiftDefinitionId { get; set; }
    public Guid MineSiteId { get; set; }
    public DateOnly Date { get; set; }
    public string? SupervisorName { get; set; }
    public Guid? SupervisorId { get; set; }
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public int? PersonnelCount { get; set; }
    public string? WeatherConditions { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public ShiftDefinition ShiftDefinition { get; set; } = default!;
    public MineSite MineSite { get; set; } = default!;
    public ICollection<ShiftHandover> Handovers { get; set; } = new List<ShiftHandover>();
}
