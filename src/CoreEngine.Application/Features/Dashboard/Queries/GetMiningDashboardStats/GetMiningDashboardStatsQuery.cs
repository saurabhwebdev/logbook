using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Dashboard.Queries.GetMiningDashboardStats;

public record MiningDashboardStatsDto(
    // Mine Sites
    int TotalMineSites,
    int ActiveMineSites,

    // Safety
    int TotalIncidents,
    int OpenIncidents,
    int IncidentsThisMonth,
    int LostTimeDays,

    // Inspections
    int TotalInspections,
    int OverdueInspections,
    int OpenFindings,

    // Equipment
    int TotalEquipment,
    int OperationalEquipment,
    int UnderMaintenanceEquipment,
    int OverdueMaintenanceCount,

    // Personnel
    int TotalPersonnel,
    int ActivePersonnel,
    int ExpiringCertifications,

    // Production
    decimal TotalProductionTonnes,
    decimal ProductionThisMonth,
    int DispatchCount,
    int DispatchThisMonth,

    // Blasting
    int TotalBlasts,
    int BlastsThisMonth,
    decimal TotalExplosivesUsedKg,

    // Permits
    int ActivePermits,
    int PendingPermits,
    int ExpiredPermits,

    // Environmental
    int EnvironmentalExceedances,
    int OpenEnvironmentalIncidents,

    // Ventilation
    int GasExceedances,
    int CriticalVentilationReadings,

    // Compliance
    int TotalRequirements,
    int CompliantCount,
    int NonCompliantCount,
    int OverdueAudits,

    // Geotechnical
    int UnstableAssessments,
    int PendingSurveys,

    // Recent items
    List<RecentMiningActivityDto> RecentSafetyIncidents,
    List<RecentMiningActivityDto> RecentInspections,
    List<RecentMiningActivityDto> RecentPermits
);

public record RecentMiningActivityDto(
    string Id,
    string Title,
    string Status,
    string Severity,
    DateTime Date
);

public record GetMiningDashboardStatsQuery : IRequest<MiningDashboardStatsDto>;

public class GetMiningDashboardStatsQueryHandler : IRequestHandler<GetMiningDashboardStatsQuery, MiningDashboardStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetMiningDashboardStatsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiningDashboardStatsDto> Handle(GetMiningDashboardStatsQuery request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // Mine Sites
        var totalMineSites = await _context.MineSites.CountAsync(ct);
        var activeMineSites = await _context.MineSites.CountAsync(s => s.Status == "Active", ct);

        // Safety
        var totalIncidents = await _context.SafetyIncidents.CountAsync(ct);
        var openIncidents = await _context.SafetyIncidents.CountAsync(i => i.Status != "Closed" && i.Status != "Resolved", ct);
        var incidentsThisMonth = await _context.SafetyIncidents.CountAsync(i => i.IncidentDateTime >= startOfMonth, ct);
        var lostTimeDays = (int)(await _context.SafetyIncidents
            .Where(i => i.LostTimeDays.HasValue)
            .SumAsync(i => (int)i.LostTimeDays!.Value, ct));

        // Inspections
        var totalInspections = await _context.Inspections.CountAsync(ct);
        var overdueInspections = await _context.Inspections.CountAsync(i => i.Status == "Scheduled" && i.ScheduledDate < now, ct);
        var openFindings = await _context.InspectionFindings.CountAsync(f => f.Status == "Open" || f.Status == "InProgress", ct);

        // Equipment
        var totalEquipment = await _context.Equipment.CountAsync(ct);
        var operationalEquipment = await _context.Equipment.CountAsync(e => e.Status == "Operational", ct);
        var underMaintenanceEquipment = await _context.Equipment.CountAsync(e => e.Status == "UnderMaintenance", ct);
        var overdueMaintenanceCount = await _context.MaintenanceRecords.CountAsync(m =>
            m.Status == "Scheduled" && m.ScheduledDate < now, ct);

        // Personnel
        var totalPersonnel = await _context.Personnel.CountAsync(ct);
        var activePersonnel = await _context.Personnel.CountAsync(p => p.Status == "Active", ct);
        var thirtyDaysFromNow = now.AddDays(30);
        var expiringCertifications = await _context.PersonnelCertifications.CountAsync(c =>
            c.ExpiryDate.HasValue && c.ExpiryDate.Value <= thirtyDaysFromNow && c.ExpiryDate.Value >= now && c.Status == "Active", ct);

        // Production
        var totalProductionTonnes = await _context.ProductionLogs.SumAsync(p => p.QuantityTonnes, ct);
        var productionThisMonth = await _context.ProductionLogs
            .Where(p => p.Date >= startOfMonth)
            .SumAsync(p => p.QuantityTonnes, ct);
        var dispatchCount = await _context.DispatchRecords.CountAsync(ct);
        var dispatchThisMonth = await _context.DispatchRecords.CountAsync(d => d.Date >= startOfMonth, ct);

        // Blasting
        var totalBlasts = await _context.BlastEvents.CountAsync(ct);
        var blastsThisMonth = await _context.BlastEvents.CountAsync(b => b.ScheduledDateTime >= startOfMonth, ct);
        var totalExplosivesUsedKg = await _context.ExplosiveUsages.SumAsync(e => e.QuantityUsed, ct);

        // Permits
        var activePermits = await _context.WorkPermits.CountAsync(p => p.Status == "Active" || p.Status == "Approved", ct);
        var pendingPermits = await _context.WorkPermits.CountAsync(p => p.Status == "Pending" || p.Status == "Draft", ct);
        var expiredPermits = await _context.WorkPermits.CountAsync(p => p.Status == "Expired", ct);

        // Environmental
        var environmentalExceedances = await _context.EnvironmentalReadings.CountAsync(r => r.IsExceedance, ct);
        var openEnvironmentalIncidents = await _context.EnvironmentalIncidents.CountAsync(i =>
            i.Status != "Closed" && i.Status != "Resolved", ct);

        // Ventilation & Gas
        var gasExceedances = await _context.GasReadings.CountAsync(g => g.IsExceedance, ct);
        var criticalVentilationReadings = await _context.VentilationReadings.CountAsync(v =>
            v.VentilationStatus == "Critical" || v.VentilationStatus == "Warning", ct);

        // Compliance
        var totalRequirements = await _context.ComplianceRequirements.CountAsync(r => r.IsActive, ct);
        var compliantCount = await _context.ComplianceRequirements.CountAsync(r => r.IsActive && r.Status == "Compliant", ct);
        var nonCompliantCount = await _context.ComplianceRequirements.CountAsync(r => r.IsActive && r.Status == "NonCompliant", ct);
        var overdueAudits = await _context.ComplianceRequirements.CountAsync(r =>
            r.IsActive && r.NextDueDate.HasValue && r.NextDueDate.Value < now, ct);

        // Geotechnical
        var unstableAssessments = await _context.GeotechnicalAssessments.CountAsync(a =>
            a.StabilityStatus == "Unstable" || a.StabilityStatus == "Critical", ct);
        var pendingSurveys = await _context.SurveyRecords.CountAsync(s => s.Status == "Pending" || s.Status == "InProgress", ct);

        // Recent Safety Incidents
        var recentSafetyIncidents = await _context.SafetyIncidents
            .OrderByDescending(i => i.IncidentDateTime)
            .Take(5)
            .Select(i => new RecentMiningActivityDto(i.Id.ToString(), i.Title, i.Status, i.Severity, i.IncidentDateTime))
            .ToListAsync(ct);

        // Recent Inspections
        var recentInspections = await _context.Inspections
            .OrderByDescending(i => i.ScheduledDate)
            .Take(5)
            .Select(i => new RecentMiningActivityDto(i.Id.ToString(), i.Title, i.Status, i.OverallRating ?? "", i.ScheduledDate))
            .ToListAsync(ct);

        // Recent Permits
        var recentPermits = await _context.WorkPermits
            .OrderByDescending(p => p.RequestDate)
            .Take(5)
            .Select(p => new RecentMiningActivityDto(p.Id.ToString(), p.Title, p.Status, p.PermitType, p.RequestDate))
            .ToListAsync(ct);

        return new MiningDashboardStatsDto(
            totalMineSites, activeMineSites,
            totalIncidents, openIncidents, incidentsThisMonth, lostTimeDays,
            totalInspections, overdueInspections, openFindings,
            totalEquipment, operationalEquipment, underMaintenanceEquipment, overdueMaintenanceCount,
            totalPersonnel, activePersonnel, expiringCertifications,
            totalProductionTonnes, productionThisMonth, dispatchCount, dispatchThisMonth,
            totalBlasts, blastsThisMonth, totalExplosivesUsedKg,
            activePermits, pendingPermits, expiredPermits,
            environmentalExceedances, openEnvironmentalIncidents,
            gasExceedances, criticalVentilationReadings,
            totalRequirements, compliantCount, nonCompliantCount, overdueAudits,
            unstableAssessments, pendingSurveys,
            recentSafetyIncidents, recentInspections, recentPermits
        );
    }
}
