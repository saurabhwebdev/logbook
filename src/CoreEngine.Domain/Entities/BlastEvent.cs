using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class BlastEvent : TenantScopedEntity
{
    public Guid MineSiteId { get; set; }
    public Guid? MineAreaId { get; set; }
    public string BlastNumber { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string BlastType { get; set; } = default!; // Development, Production, Secondary, Presplit, TrimBlast, Other
    public DateTime ScheduledDateTime { get; set; }
    public DateTime? ActualDateTime { get; set; }
    public string Location { get; set; } = default!;
    public string? DrillingPattern { get; set; }
    public int? NumberOfHoles { get; set; }
    public decimal? TotalExplosivesKg { get; set; }
    public string? ExplosiveType { get; set; }
    public string? DetonatorType { get; set; }
    public string Status { get; set; } = "Planned"; // Planned, Approved, InProgress, Completed, Cancelled, Misfired
    public string? BlastDesignNotes { get; set; }
    public double? SafetyRadius { get; set; }
    public bool EvacuationConfirmed { get; set; }
    public bool SentryPostsConfirmed { get; set; }
    public bool PreBlastWarningGiven { get; set; }
    public string SupervisorName { get; set; } = default!;
    public string LicensedBlasterName { get; set; } = default!;
    public double? VibrationReading { get; set; }
    public double? AirBlastReading { get; set; }
    public string? PostBlastInspection { get; set; }
    public string? PostBlastNotes { get; set; }
    public string? FragmentationQuality { get; set; } // Excellent, Good, Fair, Poor
    public int MisfireCount { get; set; }

    // Navigation
    public MineSite MineSite { get; set; } = default!;
    public MineArea? MineArea { get; set; }
    public ICollection<ExplosiveUsage> ExplosiveUsages { get; set; } = new List<ExplosiveUsage>();
}
