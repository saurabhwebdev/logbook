using CoreEngine.Domain.Common;

namespace CoreEngine.Domain.Entities;

public class MineSite : TenantScopedEntity
{
    public string Name { get; set; } = default!;
    public string? Code { get; set; }
    public string MineType { get; set; } = default!; // Underground, OpenPit, Mixed
    public string Jurisdiction { get; set; } = default!; // MSHA, DGMS, AU_QLD, AU_NSW, SA_MHSA, CANADA_BC, CHILE_SERNAGEOMIN, OTHER
    public string? JurisdictionDetails { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? MineralsMined { get; set; }
    public string? OperatingCompany { get; set; }
    public string? MiningLicenseNumber { get; set; }
    public DateTime? LicenseExpiryDate { get; set; }
    public DateTime? OperationalSince { get; set; }
    public string Status { get; set; } = "Active"; // Active, Suspended, Closed, UnderConstruction
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? NearestHospital { get; set; }
    public string? NearestHospitalPhone { get; set; }
    public double? NearestHospitalDistanceKm { get; set; }
    public string UnitSystem { get; set; } = "Metric"; // Metric, Imperial
    public string TimeZone { get; set; } = "UTC";
    public int ShiftsPerDay { get; set; } = 3;
    public string? ShiftPattern { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = default!;
    public ICollection<MineArea> MineAreas { get; set; } = new List<MineArea>();
}
