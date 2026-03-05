using CoreEngine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Infrastructure.Persistence.Seed;

public static class MiningDemoDataSeeder
{
    private static readonly Guid DefaultTenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static async Task<string> SeedModuleAsync(ApplicationDbContext context, string module)
    {
        return module.ToLower() switch
        {
            "minesites" => await SeedMineSitesAsync(context),
            "shifts" => await SeedShiftsAsync(context),
            "registers" => await SeedRegistersAsync(context),
            "safety" => await SeedSafetyAsync(context),
            "inspections" => await SeedInspectionsAsync(context),
            "equipment" => await SeedEquipmentAsync(context),
            "personnel" => await SeedPersonnelAsync(context),
            "blasting" => await SeedBlastingAsync(context),
            "production" => await SeedProductionAsync(context),
            "permits" => await SeedPermitsAsync(context),
            "environmental" => await SeedEnvironmentalAsync(context),
            "ventilation" => await SeedVentilationAsync(context),
            "compliance" => await SeedComplianceAsync(context),
            "geotechnical" => await SeedGeotechnicalAsync(context),
            "all" => await SeedAllAsync(context),
            _ => throw new ArgumentException($"Unknown module: {module}")
        };
    }

    private static async Task<string> SeedAllAsync(ApplicationDbContext context)
    {
        var results = new List<string>
        {
            await SeedMineSitesAsync(context),
            await SeedShiftsAsync(context),
            await SeedRegistersAsync(context),
            await SeedSafetyAsync(context),
            await SeedInspectionsAsync(context),
            await SeedEquipmentAsync(context),
            await SeedPersonnelAsync(context),
            await SeedBlastingAsync(context),
            await SeedProductionAsync(context),
            await SeedPermitsAsync(context),
            await SeedEnvironmentalAsync(context),
            await SeedVentilationAsync(context),
            await SeedComplianceAsync(context),
            await SeedGeotechnicalAsync(context)
        };
        return string.Join("\n", results);
    }

    // ============================================
    // 1. Mine Sites & Areas
    // ============================================
    private static async Task<string> SeedMineSitesAsync(ApplicationDbContext context)
    {
        if (await context.MineSites.AnyAsync()) return "MineSites: Already seeded.";

        var site1Id = Guid.NewGuid();
        var site2Id = Guid.NewGuid();

        var sites = new[]
        {
            new MineSite
            {
                Id = site1Id, Name = "Boddington Gold Mine", Code = "BGM-01",
                MineType = "OpenPit", Jurisdiction = "AU_WA",
                JurisdictionDetails = "Department of Mines, Industry Regulation and Safety (DMIRS)",
                Latitude = -32.7475, Longitude = 116.3828,
                Address = "958 Boddington-Marradong Rd", Country = "Australia", State = "Western Australia",
                MineralsMined = "Gold, Copper", OperatingCompany = "Newmont Corporation",
                MiningLicenseNumber = "ML-70/1234", LicenseExpiryDate = new DateTime(2035, 6, 30),
                OperationalSince = new DateTime(2009, 7, 1),
                Status = "Active",
                EmergencyContactName = "David Thompson", EmergencyContactPhone = "+61 8 9733 5500",
                NearestHospital = "Murray District Hospital", NearestHospitalPhone = "+61 8 9733 1222",
                NearestHospitalDistanceKm = 28.5,
                UnitSystem = "Metric", TimeZone = "Australia/Perth", ShiftsPerDay = 2,
                ShiftPattern = "7 on / 7 off FIFO",
                TenantId = DefaultTenantId
            },
            new MineSite
            {
                Id = site2Id, Name = "Kalgoorlie Super Pit", Code = "KSP-01",
                MineType = "OpenPit", Jurisdiction = "AU_WA",
                JurisdictionDetails = "DMIRS Western Australia",
                Latitude = -30.7744, Longitude = 121.5044,
                Address = "Goldfields Highway, Fimiston", Country = "Australia", State = "Western Australia",
                MineralsMined = "Gold", OperatingCompany = "Northern Star Resources",
                MiningLicenseNumber = "ML-26/5678", LicenseExpiryDate = new DateTime(2040, 12, 31),
                OperationalSince = new DateTime(1989, 1, 15),
                Status = "Active",
                EmergencyContactName = "Sarah Mitchell", EmergencyContactPhone = "+61 8 9022 1100",
                NearestHospital = "Kalgoorlie Health Campus", NearestHospitalPhone = "+61 8 9080 5888",
                NearestHospitalDistanceKm = 4.2,
                UnitSystem = "Metric", TimeZone = "Australia/Perth", ShiftsPerDay = 2,
                ShiftPattern = "14 on / 7 off",
                TenantId = DefaultTenantId
            }
        };
        context.MineSites.AddRange(sites);
        await context.SaveChangesAsync();

        // Areas for Boddington
        var areas = new[]
        {
            new MineArea { MineSiteId = site1Id, Name = "Wandoo North Pit", Code = "WNP", AreaType = "Pit", Description = "Primary gold extraction pit - northern section", Elevation = 285, SortOrder = 1, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "Wandoo South Pit", Code = "WSP", AreaType = "Pit", Description = "Southern extension pit - copper-gold ore", Elevation = 270, SortOrder = 2, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "Processing Plant", Code = "PP", AreaType = "ProcessingPlant", Description = "SAG/Ball mill circuit with gravity gold recovery", Elevation = 310, SortOrder = 3, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "ROM Stockpile", Code = "ROM", AreaType = "Stockpile", Description = "Run-of-mine ore stockpile area", Elevation = 300, SortOrder = 4, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "Waste Dump - North", Code = "WDN", AreaType = "WasteDump", Description = "Northern waste rock dump - non-acid forming", Elevation = 340, SortOrder = 5, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "TSF Dam 1", Code = "TSF1", AreaType = "TailingsDam", Description = "Tailings storage facility - primary", Elevation = 295, SortOrder = 6, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "Magazine Storage", Code = "MAG", AreaType = "Magazine", Description = "Explosive magazine and detonator storage", Elevation = 320, SortOrder = 7, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site1Id, Name = "Workshop Complex", Code = "WKS", AreaType = "Workshop", Description = "Heavy vehicle maintenance workshop and wash bay", Elevation = 305, SortOrder = 8, TenantId = DefaultTenantId },
            // Areas for Kalgoorlie
            new MineArea { MineSiteId = site2Id, Name = "Fimiston Pit", Code = "FP", AreaType = "Pit", Description = "Main open pit - 3.5km long, 1.5km wide, 600m deep", Elevation = 0, SortOrder = 1, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site2Id, Name = "Mt Charlotte UG", Code = "MCUG", AreaType = "Decline", Description = "Underground operations via decline access", Elevation = -450, SortOrder = 2, TenantId = DefaultTenantId },
            new MineArea { MineSiteId = site2Id, Name = "Fimiston Mill", Code = "FMILL", AreaType = "ProcessingPlant", Description = "12 Mtpa processing facility with CIL circuit", Elevation = 350, SortOrder = 3, TenantId = DefaultTenantId },
        };
        context.MineAreas.AddRange(areas);
        await context.SaveChangesAsync();

        return "MineSites: Seeded 2 mine sites + 11 mine areas.";
    }

    // ============================================
    // 2. Shift Definitions & Instances
    // ============================================
    private static async Task<string> SeedShiftsAsync(ApplicationDbContext context)
    {
        if (await context.ShiftDefinitions.AnyAsync()) return "Shifts: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Shifts: No mine site found. Seed MineSites first.";

        var dayShiftId = Guid.NewGuid();
        var nightShiftId = Guid.NewGuid();

        var defs = new[]
        {
            new ShiftDefinition { Id = dayShiftId, MineSiteId = site.Id, Name = "Day Shift", Code = "DS", StartTime = new TimeSpan(6, 0, 0), EndTime = new TimeSpan(18, 0, 0), ShiftOrder = 1, Color = "#ff9500", TenantId = DefaultTenantId },
            new ShiftDefinition { Id = nightShiftId, MineSiteId = site.Id, Name = "Night Shift", Code = "NS", StartTime = new TimeSpan(18, 0, 0), EndTime = new TimeSpan(6, 0, 0), ShiftOrder = 2, Color = "#5856d6", TenantId = DefaultTenantId },
        };
        context.ShiftDefinitions.AddRange(defs);
        await context.SaveChangesAsync();

        var today = DateTime.UtcNow.Date;
        var instances = new List<ShiftInstance>();
        var handovers = new List<ShiftHandover>();

        for (int i = 0; i < 7; i++)
        {
            var date = DateOnly.FromDateTime(today.AddDays(-i));
            var dayInst = new ShiftInstance
            {
                Id = Guid.NewGuid(), ShiftDefinitionId = dayShiftId, MineSiteId = site.Id,
                Date = date, SupervisorName = i % 2 == 0 ? "Mike Henderson" : "Craig Patterson",
                Status = i == 0 ? "InProgress" : "Completed",
                ActualStartTime = today.AddDays(-i).AddHours(6).AddMinutes(i % 3 * 2),
                ActualEndTime = i == 0 ? null : today.AddDays(-i).AddHours(18).AddMinutes(5),
                PersonnelCount = 45 + i % 5, WeatherConditions = i % 3 == 0 ? "Clear, 32°C" : i % 3 == 1 ? "Partly cloudy, 28°C" : "Light rain, 24°C",
                Notes = i == 0 ? "Current shift in progress" : null,
                TenantId = DefaultTenantId
            };
            var nightInst = new ShiftInstance
            {
                Id = Guid.NewGuid(), ShiftDefinitionId = nightShiftId, MineSiteId = site.Id,
                Date = date, SupervisorName = i % 2 == 0 ? "Tony Nguyen" : "Lisa Campbell",
                Status = i == 0 ? "Scheduled" : "Completed",
                ActualStartTime = i == 0 ? null : today.AddDays(-i).AddHours(18).AddMinutes(3),
                ActualEndTime = i == 0 ? null : today.AddDays(-i + 1).AddHours(6).AddMinutes(8),
                PersonnelCount = 38 + i % 4,
                TenantId = DefaultTenantId
            };
            instances.Add(dayInst);
            instances.Add(nightInst);

            if (i > 0)
            {
                handovers.Add(new ShiftHandover
                {
                    OutgoingShiftInstanceId = dayInst.Id, IncomingShiftInstanceId = nightInst.Id,
                    MineSiteId = site.Id,
                    HandoverDateTime = today.AddDays(-i).AddHours(18),
                    SafetyIssues = i == 1 ? "Loose ground reported on Bench 4 in Wandoo North - barricaded and geologist notified" : null,
                    OngoingOperations = "Hauling from WNP Bench 6 to ROM pad. Drill rig 3 relocating to WSP.",
                    PendingTasks = i % 2 == 0 ? "Dewatering pump #2 needs filter replacement" : "Blast clearance required for tomorrow morning shot",
                    EquipmentStatus = "All trucks operational. Excavator EX-003 scheduled service tomorrow.",
                    EnvironmentalConditions = i % 2 == 0 ? "Dust suppression active. Wind from NW at 15 km/h." : "Light rain expected overnight. Monitor pit sumps.",
                    HandedOverBy = dayInst.SupervisorName, ReceivedBy = nightInst.SupervisorName,
                    Status = "Acknowledged", AcknowledgedAt = today.AddDays(-i).AddHours(18).AddMinutes(15),
                    TenantId = DefaultTenantId
                });
            }
        }
        context.ShiftInstances.AddRange(instances);
        await context.SaveChangesAsync();
        context.ShiftHandovers.AddRange(handovers);
        await context.SaveChangesAsync();

        return "Shifts: Seeded 2 definitions, 14 instances, 6 handovers.";
    }

    // ============================================
    // 3. Statutory Registers
    // ============================================
    private static async Task<string> SeedRegistersAsync(ApplicationDbContext context)
    {
        if (await context.StatutoryRegisters.AnyAsync()) return "Registers: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Registers: No mine site found.";

        var accidentRegId = Guid.NewGuid();
        var explosivesRegId = Guid.NewGuid();

        var registers = new[]
        {
            new StatutoryRegister { Id = accidentRegId, MineSiteId = site.Id, Name = "Accident & Dangerous Occurrence Register", Code = "REG-ADO", RegisterType = "AccidentRegister", Description = "WA Mines Safety & Inspection Regulations 1995 - Reg 9.25", Jurisdiction = "AU_WA", IsRequired = true, RetentionYears = 7, SortOrder = 1, TenantId = DefaultTenantId },
            new StatutoryRegister { Id = explosivesRegId, MineSiteId = site.Id, Name = "Explosives Magazine Register", Code = "REG-EXP", RegisterType = "ExplosivesRegister", Description = "Dangerous Goods Safety (Explosives) Regulations 2007", Jurisdiction = "AU_WA", IsRequired = true, RetentionYears = 10, SortOrder = 2, TenantId = DefaultTenantId },
            new StatutoryRegister { MineSiteId = site.Id, Name = "Abandoned Mines Register", Code = "REG-ABM", RegisterType = "GeneralRegister", Description = "Mining Rehabilitation Fund Act 2012", Jurisdiction = "AU_WA", IsRequired = true, RetentionYears = 99, SortOrder = 3, TenantId = DefaultTenantId },
            new StatutoryRegister { MineSiteId = site.Id, Name = "Training & Competency Register", Code = "REG-TRN", RegisterType = "TrainingRegister", Description = "Records of all inductions, training, and assessments", Jurisdiction = "AU_WA", IsRequired = true, RetentionYears = 7, SortOrder = 4, TenantId = DefaultTenantId },
        };
        context.StatutoryRegisters.AddRange(registers);
        await context.SaveChangesAsync();

        var area = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WNP");
        var entries = new[]
        {
            new RegisterEntry { StatutoryRegisterId = accidentRegId, MineSiteId = site.Id, MineAreaId = area?.Id, EntryNumber = 1, EntryDate = DateTime.UtcNow.AddDays(-45), Subject = "Near miss - rock fall on haul road", Details = "Loose boulder dislodged from Bench 4 wall onto haul road. No injuries. Road closed for 2 hours for scaling.", ReportedBy = "Mike Henderson", WitnessName = "James O'Brien", ActionTaken = "Geotechnical assessment ordered. Scaling crew deployed.", ActionDueDate = DateTime.UtcNow.AddDays(-40), ActionCompletedDate = DateTime.UtcNow.AddDays(-42), Status = "Closed", TenantId = DefaultTenantId },
            new RegisterEntry { StatutoryRegisterId = accidentRegId, MineSiteId = site.Id, MineAreaId = area?.Id, EntryNumber = 2, EntryDate = DateTime.UtcNow.AddDays(-20), Subject = "First aid - laceration to left hand", Details = "Operator sustained 3cm laceration while replacing bucket teeth. First aid administered on site. No lost time.", ReportedBy = "Craig Patterson", ActionTaken = "Toolbox talk conducted on correct PPE use during bucket teeth replacement", ActionDueDate = DateTime.UtcNow.AddDays(-15), ActionCompletedDate = DateTime.UtcNow.AddDays(-18), Status = "Closed", TenantId = DefaultTenantId },
            new RegisterEntry { StatutoryRegisterId = accidentRegId, MineSiteId = site.Id, EntryNumber = 3, EntryDate = DateTime.UtcNow.AddDays(-5), Subject = "Vehicle interaction - light vehicle and haul truck", Details = "Light vehicle entered active haul road without radio call. Haul truck operator took evasive action. No contact made.", ReportedBy = "Tony Nguyen", WitnessName = "Sarah Chen, Paul Roberts", ActionTaken = "Driver counselled. Review of traffic management plan initiated.", ActionDueDate = DateTime.UtcNow.AddDays(5), Status = "Open", TenantId = DefaultTenantId },
            new RegisterEntry { StatutoryRegisterId = explosivesRegId, MineSiteId = site.Id, EntryNumber = 1, EntryDate = DateTime.UtcNow.AddDays(-30), Subject = "Monthly explosive stock reconciliation - February 2026", Details = "Opening stock: 4500kg ANFO, 120 det cords, 85 boosters. Received: 2000kg ANFO. Used: 3200kg ANFO, 45 det cords, 30 boosters. Closing stock: 3300kg ANFO, 75 det cords, 55 boosters.", ReportedBy = "Mark Sullivan", Status = "Closed", TenantId = DefaultTenantId },
        };
        context.RegisterEntries.AddRange(entries);
        await context.SaveChangesAsync();

        return "Registers: Seeded 4 registers + 4 entries.";
    }

    // ============================================
    // 4. Safety & Incidents
    // ============================================
    private static async Task<string> SeedSafetyAsync(ApplicationDbContext context)
    {
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Safety: No mine site found.";
        var pitArea = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WNP");

        var incidentsExist = await context.SafetyIncidents.AnyAsync();
        var investigationsExist = await context.IncidentInvestigations.AnyAsync();
        if (incidentsExist && investigationsExist) return "Safety: Already seeded.";

        var inc1Id = Guid.NewGuid();
        if (!incidentsExist)
        {
        var incidents = new[]
        {
            new SafetyIncident
            {
                Id = inc1Id, MineSiteId = site.Id, MineAreaId = pitArea?.Id,
                IncidentNumber = "INC-00001", Title = "Rock fall near haul road - Bench 4",
                IncidentType = "NearMiss", Severity = "High",
                IncidentDateTime = DateTime.UtcNow.AddDays(-45), Location = "Wandoo North Pit, Bench 4 access ramp",
                Description = "A 0.5m³ boulder dislodged from the pit wall and fell approximately 8m onto the haul road. The road was active but no vehicles were in the immediate area at the time. The fall occurred during dayshift at approximately 14:30.",
                ImmediateActions = "Area cordoned off. Haul road rerouted via alternate access. Rock scaling crew mobilised.",
                ReportedBy = "Mike Henderson", ReportedAt = DateTime.UtcNow.AddDays(-45).AddHours(1),
                WitnessNames = "James O'Brien (Truck driver), Kevin Lee (Spotter)",
                RootCause = "Weathering of contact zone between laterite and fresh rock. Recent heavy rainfall exacerbated the condition.",
                ContributingFactors = "Rainfall event 48 hours prior. Delayed geotechnical inspection schedule.",
                CorrectiveActions = "1. Geotechnical reassessment of Bench 4 face. 2. Install ground movement monitoring prisms. 3. Review scaling frequency.",
                CorrectiveActionDueDate = DateTime.UtcNow.AddDays(-30),
                CorrectiveActionCompletedDate = DateTime.UtcNow.AddDays(-35),
                IsReportable = true, RegulatoryReference = "MSIR Reg 9.25(1)(e)",
                Status = "Closed", TenantId = DefaultTenantId
            },
            new SafetyIncident
            {
                MineSiteId = site.Id, IncidentNumber = "INC-00002",
                Title = "First aid - hand laceration during maintenance",
                IncidentType = "FirstAid", Severity = "Low",
                IncidentDateTime = DateTime.UtcNow.AddDays(-20), Location = "Workshop Complex",
                Description = "Maintenance fitter sustained a 3cm laceration to the left palm while replacing bucket teeth on excavator EX-002. The fitter was wearing standard work gloves but not impact-rated gloves.",
                ImmediateActions = "First aid administered. Wound cleaned and dressed. Fitter returned to modified duties.",
                ReportedBy = "Craig Patterson", ReportedAt = DateTime.UtcNow.AddDays(-20).AddMinutes(45),
                InjuredPersonName = "Daniel Cooper", InjuredPersonRole = "Maintenance Fitter",
                InjuryType = "Laceration", BodyPartAffected = "Left hand",
                LostTimeDays = 0, IsReportable = false,
                CorrectiveActions = "Toolbox talk on correct glove selection. Impact-rated gloves procured for all bucket maintenance tasks.",
                CorrectiveActionDueDate = DateTime.UtcNow.AddDays(-15),
                CorrectiveActionCompletedDate = DateTime.UtcNow.AddDays(-17),
                Status = "Closed", TenantId = DefaultTenantId
            },
            new SafetyIncident
            {
                MineSiteId = site.Id, IncidentNumber = "INC-00003",
                Title = "Vehicle interaction on main haul road",
                IncidentType = "NearMiss", Severity = "Critical",
                IncidentDateTime = DateTime.UtcNow.AddDays(-5), Location = "Main haul road, intersection 3",
                Description = "A light vehicle entered the active haul road without making a radio call on Channel 4. A loaded CAT 793F haul truck was approaching from the south. The truck operator saw the LV and applied emergency braking. No contact was made. Minimum separation was approximately 15 metres.",
                ImmediateActions = "LV driver immediately stopped. Radio call made. Both drivers reported to supervisor. LV driver stood down for the shift.",
                ReportedBy = "Tony Nguyen", ReportedAt = DateTime.UtcNow.AddDays(-5).AddMinutes(30),
                WitnessNames = "Sarah Chen (Water cart operator), Paul Roberts (Spotter)",
                RootCause = "Driver complacency and failure to follow traffic management procedures.",
                CorrectiveActions = "1. Refresher training for all LV drivers on traffic management. 2. Additional signage at intersection 3. 3. Review of LV/HV interaction controls.",
                CorrectiveActionDueDate = DateTime.UtcNow.AddDays(10),
                IsReportable = true, RegulatoryReference = "MSIR Reg 9.25(1)(a)",
                Status = "UnderInvestigation", TenantId = DefaultTenantId
            },
            new SafetyIncident
            {
                MineSiteId = site.Id, MineAreaId = pitArea?.Id,
                IncidentNumber = "INC-00004", Title = "Dust exceedance at pit boundary",
                IncidentType = "EnvironmentalIncident", Severity = "Medium",
                IncidentDateTime = DateTime.UtcNow.AddDays(-2), Location = "Wandoo North Pit boundary monitoring station",
                Description = "PM10 dust monitoring station recorded 82 µg/m³ (24-hour average) exceeding the 70 µg/m³ licence limit. High winds from the NW at 35 km/h contributed to elevated dust levels.",
                ImmediateActions = "Additional water carts deployed. Reduced speed limit on unsealed roads. Notification sent to DMIRS.",
                ReportedBy = "Environmental Officer - Rachel Kim", ReportedAt = DateTime.UtcNow.AddDays(-2).AddHours(2),
                IsReportable = true, RegulatoryReference = "Environmental Licence L8765/2014",
                CorrectiveActions = "Review dust management plan triggers. Install additional sprinkler system on northern haul road.",
                CorrectiveActionDueDate = DateTime.UtcNow.AddDays(14),
                Status = "Open", TenantId = DefaultTenantId
            },
        };
        context.SafetyIncidents.AddRange(incidents);
        await context.SaveChangesAsync();
        }

        // Investigation for incident 1
        if (!investigationsExist)
        {
            var incidentForInvestigation = incidentsExist
                ? await context.SafetyIncidents.FirstAsync(i => i.IncidentNumber == "INC-00001")
                : null;
            var investigationIncidentId = incidentForInvestigation?.Id ?? inc1Id;

            var investigations = new[]
            {
                new IncidentInvestigation
                {
                    SafetyIncidentId = investigationIncidentId,
                    InvestigatorName = "Dr. Alan Whitfield",
                    InvestigationDate = DateTime.UtcNow.AddDays(-42),
                    Methodology = "IncidentCauseAnalysis",
                    Findings = "The rock fall originated from the contact zone between weathered laterite and fresh dolerite at RL 265. A 140mm rainfall event 48 hours prior to the incident saturated the weak zone. The existing scaling inspection interval of 14 days was insufficient for this area given the geological conditions.",
                    RootCauseAnalysis = "Inadequate hazard identification during geotechnical risk assessment. The contact zone was identified but rated as 'low risk' during the 2025 geotechnical review. Rainfall-triggered instability was not adequately considered.",
                    Recommendations = "1. Re-classify all laterite/fresh rock contacts as 'moderate' risk minimum.\n2. Install 4x automated prism monitoring points on Bench 4.\n3. Reduce scaling inspection interval to 7 days for weathered zones.\n4. Implement rainfall-triggered inspection protocol (>50mm in 24hrs).",
                    PreventiveMeasures = "Ground awareness training for all pit personnel. Updated geotechnical hazard map distributed.",
                    EvidenceReferences = "Photos: IMG_3421-3438. Geotech report: GR-2026-017. Witness statements: WS-001, WS-002.",
                    Status = "Completed", TenantId = DefaultTenantId
                },
            };
            context.IncidentInvestigations.AddRange(investigations);
            await context.SaveChangesAsync();
        }

        return "Safety: Seeded 4 incidents + 1 investigation.";
    }

    // ============================================
    // 5. Inspections
    // ============================================
    private static async Task<string> SeedInspectionsAsync(ApplicationDbContext context)
    {
        if (await context.InspectionTemplates.AnyAsync()) return "Inspections: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Inspections: No mine site found.";

        var wpInspId = Guid.NewGuid();
        var pitInspId = Guid.NewGuid();

        var templates = new[]
        {
            new InspectionTemplate { Id = wpInspId, Name = "Workplace Safety Inspection", Code = "WSI", Category = "Safety", Description = "General workplace safety walk-through covering housekeeping, PPE compliance, and hazard identification", Frequency = "Weekly", SortOrder = 1, TenantId = DefaultTenantId },
            new InspectionTemplate { Id = pitInspId, Name = "Pit Wall Inspection", Code = "PWI", Category = "Geotechnical", Description = "Systematic visual inspection of pit walls for signs of instability, cracking, water seepage, and undercutting", Frequency = "Fortnightly", SortOrder = 2, TenantId = DefaultTenantId },
            new InspectionTemplate { Name = "Heavy Vehicle Pre-Start", Code = "HVPS", Category = "Equipment", Description = "Pre-operational check for haul trucks and excavators", Frequency = "Daily", SortOrder = 3, TenantId = DefaultTenantId },
            new InspectionTemplate { Name = "Explosives Magazine Inspection", Code = "EMI", Category = "Regulatory", Description = "Monthly compliance inspection of explosive storage facilities per DGS regulations", Frequency = "Monthly", SortOrder = 4, TenantId = DefaultTenantId },
        };
        context.InspectionTemplates.AddRange(templates);
        await context.SaveChangesAsync();

        var area = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WNP");
        var insp1Id = Guid.NewGuid();
        var inspections = new[]
        {
            new Inspection
            {
                Id = insp1Id, InspectionTemplateId = wpInspId, MineSiteId = site.Id, MineAreaId = area?.Id,
                InspectionNumber = "INS-00001", Title = "Weekly Safety Walk - Wandoo North Pit",
                ScheduledDate = DateTime.UtcNow.AddDays(-7),
                CompletedDate = DateTime.UtcNow.AddDays(-7).AddHours(2),
                InspectorName = "Mike Henderson", InspectorRole = "Shift Supervisor",
                Status = "Completed", OverallRating = "Satisfactory",
                Summary = "General housekeeping good. Two findings raised - one re signage and one re bunding. No critical safety issues.",
                WeatherConditions = "Clear, 30°C, light NW breeze", PersonnelPresent = 12,
                SignedOffBy = "Craig Patterson", SignedOffAt = DateTime.UtcNow.AddDays(-6),
                TenantId = DefaultTenantId
            },
            new Inspection
            {
                InspectionTemplateId = pitInspId, MineSiteId = site.Id, MineAreaId = area?.Id,
                InspectionNumber = "INS-00002", Title = "Fortnightly Pit Wall Inspection - WNP",
                ScheduledDate = DateTime.UtcNow.AddDays(-3),
                CompletedDate = DateTime.UtcNow.AddDays(-3).AddHours(4),
                InspectorName = "Dr. Alan Whitfield", InspectorRole = "Principal Geotechnical Engineer",
                Status = "Completed", OverallRating = "Requires Attention",
                Summary = "Bench 4 contact zone showing improved stability since scaling. New tension crack identified on Bench 7 - monitoring installed. Water seepage on western wall at RL240.",
                WeatherConditions = "Overcast, 24°C", PersonnelPresent = 3,
                TenantId = DefaultTenantId
            },
            new Inspection
            {
                InspectionTemplateId = wpInspId, MineSiteId = site.Id,
                InspectionNumber = "INS-00003", Title = "Weekly Safety Walk - Processing Plant",
                ScheduledDate = DateTime.UtcNow.AddDays(1),
                InspectorName = "Lisa Campbell", InspectorRole = "Safety Coordinator",
                Status = "Scheduled",
                TenantId = DefaultTenantId
            },
        };
        context.Inspections.AddRange(inspections);
        await context.SaveChangesAsync();

        var findings = new[]
        {
            new InspectionFinding { InspectionId = insp1Id, FindingNumber = "FND-00001", Category = "Signage", Severity = "Low", Description = "Speed limit sign at intersection 2 faded and difficult to read", Location = "Haul road intersection 2", RecommendedAction = "Replace sign with new reflective signage", AssignedTo = "Infrastructure Team", ActionDueDate = DateTime.UtcNow.AddDays(7), Status = "Open", TenantId = DefaultTenantId },
            new InspectionFinding { InspectionId = insp1Id, FindingNumber = "FND-00002", Category = "Environmental", Severity = "Medium", Description = "Hydrocarbon bund around fuel storage at ROM pad has 50mm crack. Potential for leakage to ground.", Location = "ROM Stockpile - fuel bay", RecommendedAction = "Repair concrete bund. Inspect liner integrity.", AssignedTo = "Environmental Team", ActionDueDate = DateTime.UtcNow.AddDays(3), Status = "InProgress", TenantId = DefaultTenantId },
        };
        context.InspectionFindings.AddRange(findings);
        await context.SaveChangesAsync();

        return "Inspections: Seeded 4 templates, 3 inspections, 2 findings.";
    }

    // ============================================
    // 6. Equipment & Maintenance
    // ============================================
    private static async Task<string> SeedEquipmentAsync(ApplicationDbContext context)
    {
        if (await context.Equipment.AnyAsync()) return "Equipment: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Equipment: No mine site found.";
        var pitArea = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WNP");

        var truck1Id = Guid.NewGuid();
        var ex1Id = Guid.NewGuid();

        var equipment = new[]
        {
            new Equipment { Id = truck1Id, MineSiteId = site.Id, MineAreaId = pitArea?.Id, AssetNumber = "EQ-00001", Name = "CAT 793F Haul Truck #1", Category = "HaulTruck", Make = "Caterpillar", Model = "793F", SerialNumber = "CAT793F-2021-0451", YearOfManufacture = 2021, PurchaseDate = new DateTime(2021, 3, 15), PurchaseCost = 6500000m, Status = "Operational", Location = "Wandoo North Pit", OperatorName = "Brett Williams", HoursOperated = 18450, NextServiceHours = 19000, NextServiceDate = DateTime.UtcNow.AddDays(12), LastServiceDate = DateTime.UtcNow.AddDays(-30), TenantId = DefaultTenantId },
            new Equipment { MineSiteId = site.Id, MineAreaId = pitArea?.Id, AssetNumber = "EQ-00002", Name = "CAT 793F Haul Truck #2", Category = "HaulTruck", Make = "Caterpillar", Model = "793F", SerialNumber = "CAT793F-2021-0452", YearOfManufacture = 2021, PurchaseCost = 6500000m, Status = "Operational", Location = "Wandoo North Pit", HoursOperated = 17890, NextServiceHours = 18500, LastServiceDate = DateTime.UtcNow.AddDays(-15), TenantId = DefaultTenantId },
            new Equipment { MineSiteId = site.Id, MineAreaId = pitArea?.Id, AssetNumber = "EQ-00003", Name = "CAT 793F Haul Truck #3", Category = "HaulTruck", Make = "Caterpillar", Model = "793F", Status = "UnderMaintenance", Location = "Workshop", HoursOperated = 19200, Notes = "Engine overhaul in progress - estimated 5 days", TenantId = DefaultTenantId },
            new Equipment { Id = ex1Id, MineSiteId = site.Id, MineAreaId = pitArea?.Id, AssetNumber = "EQ-00004", Name = "Liebherr R 9800 Excavator", Category = "Excavator", Make = "Liebherr", Model = "R 9800", SerialNumber = "LH9800-2020-0087", YearOfManufacture = 2020, PurchaseCost = 12000000m, Status = "Operational", Location = "Wandoo North Pit - Bench 6", HoursOperated = 22100, NextServiceHours = 22500, NextServiceDate = DateTime.UtcNow.AddDays(8), LastServiceDate = DateTime.UtcNow.AddDays(-22), TenantId = DefaultTenantId },
            new Equipment { MineSiteId = site.Id, AssetNumber = "EQ-00005", Name = "Atlas Copco D65 Drill Rig", Category = "DrillRig", Make = "Atlas Copco", Model = "D65", Status = "Operational", Location = "Wandoo South Pit", HoursOperated = 8900, TenantId = DefaultTenantId },
            new Equipment { MineSiteId = site.Id, AssetNumber = "EQ-00006", Name = "CAT D10T Dozer", Category = "Dozer", Make = "Caterpillar", Model = "D10T", Status = "Operational", Location = "ROM Stockpile", HoursOperated = 14300, TenantId = DefaultTenantId },
            new Equipment { MineSiteId = site.Id, AssetNumber = "EQ-00007", Name = "Volvo A45G Articulated Truck", Category = "ArticulatedTruck", Make = "Volvo", Model = "A45G", Status = "Operational", Location = "Waste Dump North", HoursOperated = 11200, TenantId = DefaultTenantId },
            new Equipment { MineSiteId = site.Id, AssetNumber = "EQ-00008", Name = "Komatsu WA900 Wheel Loader", Category = "WheelLoader", Make = "Komatsu", Model = "WA900-8", Status = "StandBy", Location = "ROM Stockpile", HoursOperated = 9800, Notes = "On standby - backup unit", TenantId = DefaultTenantId },
        };
        context.Equipment.AddRange(equipment);
        await context.SaveChangesAsync();

        var maintenance = new[]
        {
            new MaintenanceRecord { EquipmentId = truck1Id, WorkOrderNumber = "WO-00001", MaintenanceType = "Preventive", Priority = "Medium", Title = "500-hour service - Truck #1", Description = "Scheduled 500-hour preventive maintenance including oil change, filter replacement, and brake inspection", ScheduledDate = DateTime.UtcNow.AddDays(-30), StartedAt = DateTime.UtcNow.AddDays(-30).AddHours(6), CompletedAt = DateTime.UtcNow.AddDays(-30).AddHours(14), PerformedBy = "Daniel Cooper", Status = "Completed", DowntimeHours = 8, LaborCost = 1200m, PartsCost = 3500m, PartsUsed = "Engine oil (120L), oil filter, fuel filters (2), air cleaner element, brake pads (rear)", ActionsTaken = "All filters replaced. Oil changed. Brakes inspected - 60% remaining. No defects found.", TenantId = DefaultTenantId },
            new MaintenanceRecord { EquipmentId = ex1Id, WorkOrderNumber = "WO-00002", MaintenanceType = "Corrective", Priority = "High", Title = "Hydraulic leak repair - Excavator R9800", Description = "Hydraulic oil leak identified at boom cylinder seal. Immediate repair required.", ScheduledDate = DateTime.UtcNow.AddDays(-10), StartedAt = DateTime.UtcNow.AddDays(-10).AddHours(8), CompletedAt = DateTime.UtcNow.AddDays(-9).AddHours(16), PerformedBy = "Ryan Mitchell", Status = "Completed", DowntimeHours = 32, LaborCost = 4800m, PartsCost = 12500m, PartsUsed = "Boom cylinder seal kit, hydraulic hose assembly, hydraulic oil (200L)", Findings = "Seal failure due to scoring on cylinder rod. Rod polished and new seals installed.", TenantId = DefaultTenantId },
            new MaintenanceRecord { EquipmentId = truck1Id, WorkOrderNumber = "WO-00003", MaintenanceType = "Preventive", Priority = "Medium", Title = "1000-hour service - Truck #1", ScheduledDate = DateTime.UtcNow.AddDays(12), Status = "Scheduled", TenantId = DefaultTenantId },
        };
        context.MaintenanceRecords.AddRange(maintenance);
        await context.SaveChangesAsync();

        return "Equipment: Seeded 8 equipment items + 3 maintenance records.";
    }

    // ============================================
    // 7. Personnel & Certifications
    // ============================================
    private static async Task<string> SeedPersonnelAsync(ApplicationDbContext context)
    {
        if (await context.Personnel.AnyAsync()) return "Personnel: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Personnel: No mine site found.";

        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();

        var personnel = new[]
        {
            new Personnel { Id = p1Id, MineSiteId = site.Id, EmployeeNumber = "EMP-00001", FirstName = "Mike", LastName = "Henderson", Role = "ShiftSupervisor", Department = "Mining", Designation = "Senior Shift Supervisor", EmploymentType = "Permanent", DateOfJoining = new DateTime(2015, 3, 1), Status = "Active", ContactPhone = "+61 4 1234 5678", ContactEmail = "m.henderson@bgm.com.au", EmergencyContactName = "Sarah Henderson", EmergencyContactPhone = "+61 4 9876 5432", BloodGroup = "O+", MedicalFitnessCertificate = "MED-2025-0891", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(180), TenantId = DefaultTenantId },
            new Personnel { Id = p2Id, MineSiteId = site.Id, EmployeeNumber = "EMP-00002", FirstName = "Craig", LastName = "Patterson", Role = "ShiftSupervisor", Department = "Mining", Designation = "Shift Supervisor", EmploymentType = "Permanent", DateOfJoining = new DateTime(2018, 7, 15), Status = "Active", ContactPhone = "+61 4 2345 6789", BloodGroup = "A+", MedicalFitnessCertificate = "MED-2025-0744", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(90), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00003", FirstName = "Daniel", LastName = "Cooper", Role = "MaintenanceFitter", Department = "Maintenance", EmploymentType = "Permanent", DateOfJoining = new DateTime(2019, 11, 1), Status = "Active", BloodGroup = "B+", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(250), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00004", FirstName = "Brett", LastName = "Williams", Role = "HaulTruckOperator", Department = "Mining", EmploymentType = "Permanent", DateOfJoining = new DateTime(2020, 2, 10), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(120), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00005", FirstName = "Rachel", LastName = "Kim", Role = "EnvironmentalOfficer", Department = "Environment", EmploymentType = "Permanent", DateOfJoining = new DateTime(2021, 1, 5), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(300), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00006", FirstName = "Mark", LastName = "Sullivan", Role = "ShotFirer", Department = "Drill & Blast", Designation = "Senior Shot Firer", EmploymentType = "Permanent", DateOfJoining = new DateTime(2016, 8, 20), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(60), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00007", FirstName = "Dr. Alan", LastName = "Whitfield", Role = "GeotechnicalEngineer", Department = "Technical Services", Designation = "Principal Geotechnical Engineer", EmploymentType = "Permanent", DateOfJoining = new DateTime(2014, 5, 1), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(200), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00008", FirstName = "Tony", LastName = "Nguyen", Role = "ShiftSupervisor", Department = "Mining", EmploymentType = "Permanent", DateOfJoining = new DateTime(2019, 4, 12), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(150), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00009", FirstName = "Lisa", LastName = "Campbell", Role = "SafetyCoordinator", Department = "Health & Safety", EmploymentType = "Permanent", DateOfJoining = new DateTime(2022, 3, 1), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(280), TenantId = DefaultTenantId },
            new Personnel { MineSiteId = site.Id, EmployeeNumber = "EMP-00010", FirstName = "James", LastName = "O'Brien", MiddleName = "Patrick", Role = "HaulTruckOperator", Department = "Mining", EmploymentType = "FIFO", DateOfJoining = new DateTime(2023, 1, 15), Status = "Active", MedicalFitnessExpiry = DateTime.UtcNow.AddDays(15), Notes = "Medical certificate expiring soon - renewal booked", TenantId = DefaultTenantId },
        };
        context.Personnel.AddRange(personnel);
        await context.SaveChangesAsync();

        var certs = new[]
        {
            new PersonnelCertification { PersonnelId = p1Id, CertificationName = "WA Mine Manager's Certificate of Competency", CertificateNumber = "MM-WA-2015-0234", IssuingAuthority = "DMIRS Western Australia", IssueDate = new DateTime(2015, 6, 1), Status = "Active", Category = "Statutory", TenantId = DefaultTenantId },
            new PersonnelCertification { PersonnelId = p1Id, CertificationName = "Senior First Aid", CertificateNumber = "SFA-2024-8891", IssuingAuthority = "St John Ambulance", IssueDate = new DateTime(2024, 3, 15), ExpiryDate = DateTime.UtcNow.AddDays(365), Status = "Active", Category = "Safety", TenantId = DefaultTenantId },
            new PersonnelCertification { PersonnelId = p2Id, CertificationName = "Quarry Manager Certificate", CertificateNumber = "QM-WA-2018-0567", IssuingAuthority = "DMIRS Western Australia", IssueDate = new DateTime(2018, 9, 1), Status = "Active", Category = "Statutory", TenantId = DefaultTenantId },
            new PersonnelCertification { PersonnelId = p2Id, CertificationName = "Working at Heights", CertificateNumber = "WAH-2025-1234", IssuingAuthority = "RTO National Training", IssueDate = new DateTime(2025, 1, 10), ExpiryDate = DateTime.UtcNow.AddDays(20), Status = "Active", Category = "Safety", Notes = "Expiring soon - renewal scheduled", TenantId = DefaultTenantId },
        };
        context.PersonnelCertifications.AddRange(certs);
        await context.SaveChangesAsync();

        return "Personnel: Seeded 10 personnel + 4 certifications.";
    }

    // ============================================
    // 8. Blasting & Explosives
    // ============================================
    private static async Task<string> SeedBlastingAsync(ApplicationDbContext context)
    {
        if (await context.BlastEvents.AnyAsync()) return "Blasting: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Blasting: No mine site found.";
        var area = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WNP");

        var blast1Id = Guid.NewGuid();
        var blast2Id = Guid.NewGuid();

        var blasts = new[]
        {
            new BlastEvent { Id = blast1Id, MineSiteId = site.Id, MineAreaId = area?.Id, BlastNumber = "BL-00001", Title = "WNP Bench 6 - Production blast #147", BlastType = "Production", ScheduledDateTime = DateTime.UtcNow.AddDays(-8).AddHours(12), ActualDateTime = DateTime.UtcNow.AddDays(-8).AddHours(12).AddMinutes(5), Location = "Wandoo North Pit, Bench 6, Chainage 450-520m", DrillingPattern = "6m x 7m staggered", NumberOfHoles = 85, TotalExplosivesKg = 3200, ExplosiveType = "ANFO + Emulsion boosters", DetonatorType = "Electronic (Orica i-kon III)", Status = "Completed", BlastDesignNotes = "Designed for 800mm fragmentation. Buffer row at 4.5m burden.", SafetyRadius = 500, EvacuationConfirmed = true, SentryPostsConfirmed = true, PreBlastWarningGiven = true, SupervisorName = "Mike Henderson", LicensedBlasterName = "Mark Sullivan", VibrationReading = 4.2, AirBlastReading = 115.3, PostBlastInspection = "All holes fired. No misfires. Good fragmentation achieved.", FragmentationQuality = "Good", MisfireCount = 0, TenantId = DefaultTenantId },
            new BlastEvent { Id = blast2Id, MineSiteId = site.Id, MineAreaId = area?.Id, BlastNumber = "BL-00002", Title = "WNP Bench 7 - Trim blast #22", BlastType = "Trim", ScheduledDateTime = DateTime.UtcNow.AddDays(-2).AddHours(12), ActualDateTime = DateTime.UtcNow.AddDays(-2).AddHours(12).AddMinutes(3), Location = "Wandoo North Pit, Bench 7, Western wall", DrillingPattern = "3m x 3.5m - pre-split line", NumberOfHoles = 42, TotalExplosivesKg = 850, ExplosiveType = "Emulsion + Detonating cord", DetonatorType = "Electronic (Orica i-kon III)", Status = "Completed", SafetyRadius = 500, EvacuationConfirmed = true, SentryPostsConfirmed = true, PreBlastWarningGiven = true, SupervisorName = "Craig Patterson", LicensedBlasterName = "Mark Sullivan", VibrationReading = 2.1, AirBlastReading = 108.7, PostBlastInspection = "Clean break along pre-split line. Wall condition satisfactory.", FragmentationQuality = "Excellent", MisfireCount = 0, TenantId = DefaultTenantId },
            new BlastEvent { MineSiteId = site.Id, MineAreaId = area?.Id, BlastNumber = "BL-00003", Title = "WNP Bench 6 - Production blast #148", BlastType = "Production", ScheduledDateTime = DateTime.UtcNow.AddDays(2).AddHours(12), Location = "Wandoo North Pit, Bench 6, Chainage 520-600m", DrillingPattern = "6m x 7m staggered", NumberOfHoles = 92, TotalExplosivesKg = 3500, ExplosiveType = "ANFO", DetonatorType = "Electronic", Status = "Planned", SafetyRadius = 500, SupervisorName = "Tony Nguyen", LicensedBlasterName = "Mark Sullivan", MisfireCount = 0, TenantId = DefaultTenantId },
        };
        context.BlastEvents.AddRange(blasts);
        await context.SaveChangesAsync();

        var usages = new[]
        {
            new ExplosiveUsage { BlastEventId = blast1Id, ExplosiveName = "ANFO", Type = "BulkExplosive", BatchNumber = "ANFO-2026-B044", QuantityIssued = 3000, QuantityUsed = 2850, QuantityReturned = 150, Unit = "kg", MagazineSource = "Magazine 1", IssuedBy = "Mark Sullivan", ReceivedBy = "Blast crew A", TenantId = DefaultTenantId },
            new ExplosiveUsage { BlastEventId = blast1Id, ExplosiveName = "Senatel Magnum emulsion booster", Type = "Booster", BatchNumber = "SEN-2026-0122", QuantityIssued = 100, QuantityUsed = 85, QuantityReturned = 15, Unit = "units", MagazineSource = "Magazine 1", IssuedBy = "Mark Sullivan", TenantId = DefaultTenantId },
            new ExplosiveUsage { BlastEventId = blast1Id, ExplosiveName = "i-kon III electronic detonator", Type = "Detonator", BatchNumber = "IKON-2026-0088", QuantityIssued = 90, QuantityUsed = 85, QuantityReturned = 5, Unit = "units", MagazineSource = "Magazine 2 (detonator)", IssuedBy = "Mark Sullivan", TenantId = DefaultTenantId },
            new ExplosiveUsage { BlastEventId = blast2Id, ExplosiveName = "Emulsion explosive", Type = "PackagedExplosive", QuantityIssued = 900, QuantityUsed = 850, QuantityReturned = 50, Unit = "kg", MagazineSource = "Magazine 1", IssuedBy = "Mark Sullivan", TenantId = DefaultTenantId },
        };
        context.ExplosiveUsages.AddRange(usages);
        await context.SaveChangesAsync();

        return "Blasting: Seeded 3 blast events + 4 explosive usages.";
    }

    // ============================================
    // 9. Production & Dispatch
    // ============================================
    private static async Task<string> SeedProductionAsync(ApplicationDbContext context)
    {
        if (await context.ProductionLogs.AnyAsync()) return "Production: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Production: No mine site found.";

        var logs = new List<ProductionLog>();
        var today = DateTime.UtcNow.Date;
        string[] materials = { "Gold Ore", "Copper-Gold Ore", "Waste Rock", "Oxide Ore" };
        int logNum = 1;

        for (int day = 0; day < 14; day++)
        {
            var date = today.AddDays(-day);
            var tonnes = day switch { 0 => 12000m, < 3 => 24000m + day * 500, _ => 22000m + (day % 5) * 800 };
            foreach (var shift in new[] { "Day Shift", "Night Shift" })
            {
                logs.Add(new ProductionLog
                {
                    MineSiteId = site.Id, LogNumber = $"PL-{logNum++:D5}",
                    Date = date, ShiftName = shift,
                    Material = materials[day % materials.Length],
                    SourceLocation = day % 3 == 0 ? "Wandoo North Pit - Bench 6" : "Wandoo South Pit - Bench 3",
                    DestinationLocation = day % 4 == 0 ? "Waste Dump North" : "ROM Stockpile",
                    QuantityTonnes = tonnes / 2 + (day % 3 * 200),
                    QuantityBCM = (tonnes / 2) / 2.7m,
                    EquipmentUsed = "CAT 793F x3, Liebherr R9800",
                    OperatorName = shift == "Day Shift" ? "Brett Williams" : "Jason Park",
                    HaulingDistance = 2.8 + day % 3 * 0.3,
                    LoadCount = (int)(tonnes / 2 / 220),
                    Status = day == 0 && shift == "Night Shift" ? "Draft" : "Verified",
                    VerifiedBy = day == 0 && shift == "Night Shift" ? null : "Mike Henderson",
                    VerifiedAt = day == 0 && shift == "Night Shift" ? null : date.AddHours(19),
                    TenantId = DefaultTenantId
                });
            }
        }
        context.ProductionLogs.AddRange(logs);
        await context.SaveChangesAsync();

        var dispatches = new List<DispatchRecord>();
        int dispNum = 1;
        for (int day = 0; day < 10; day++)
        {
            var date = today.AddDays(-day);
            for (int trip = 0; trip < 3; trip++)
            {
                var gross = 68m + trip * 2 + day % 3;
                var tare = 26m;
                dispatches.Add(new DispatchRecord
                {
                    MineSiteId = site.Id, DispatchNumber = $"DSP-{dispNum++:D5}",
                    Date = date, VehicleNumber = $"WA-{1200 + trip}",
                    DriverName = trip == 0 ? "Peter Johnson" : trip == 1 ? "Michael Tang" : "Andrew Smith",
                    Material = "Gold Ore Concentrate",
                    SourceLocation = "Processing Plant - Load-out",
                    DestinationLocation = "Perth Refinery - Kwinana",
                    WeighbridgeTicketNumber = $"WB-{2026000 + dispNum}",
                    GrossWeight = gross, TareWeight = tare, NetWeight = gross - tare,
                    Unit = "Tonnes",
                    DepartureTime = date.AddHours(7 + trip * 3),
                    ArrivalTime = date.AddHours(13 + trip * 3),
                    Status = day == 0 && trip == 2 ? "InTransit" : "Delivered",
                    TenantId = DefaultTenantId
                });
            }
        }
        context.DispatchRecords.AddRange(dispatches);
        await context.SaveChangesAsync();

        return $"Production: Seeded {logs.Count} production logs + {dispatches.Count} dispatch records.";
    }

    // ============================================
    // 10. Work Permits
    // ============================================
    private static async Task<string> SeedPermitsAsync(ApplicationDbContext context)
    {
        if (await context.WorkPermits.AnyAsync()) return "Permits: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Permits: No mine site found.";
        var workshopArea = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WKS");

        var permits = new[]
        {
            new WorkPermit { MineSiteId = site.Id, MineAreaId = workshopArea?.Id, PermitNumber = "PTW-00001", Title = "Hot Work - Welding repair on Truck #3 chassis", PermitType = "HotWork", RequestedBy = "Ryan Mitchell", RequestDate = DateTime.UtcNow.AddDays(-3), StartDateTime = DateTime.UtcNow.AddDays(-2).AddHours(7), EndDateTime = DateTime.UtcNow.AddDays(-2).AddHours(17), Location = "Workshop Bay 3", WorkDescription = "Welding repair on cracked chassis rail of CAT 793F #3. Includes oxy cutting of damaged section and MIG welding of repair plate.", HazardsIdentified = "Fire, UV radiation, fumes, hot metal, molten slag", ControlMeasures = "Fire watch, welding screens, LEV extraction, fire extinguisher on standby, 30-min post-work fire watch", PPERequired = "Welding helmet, leather gloves, leather apron, safety boots, hearing protection", EmergencyProcedures = "Evacuate via workshop emergency exit. Fire extinguisher locations marked. Emergency number: ext 5500", GasTestRequired = false, Status = "Completed", ApprovedBy = "Mike Henderson", ApprovedAt = DateTime.UtcNow.AddDays(-3).AddHours(16), ClosedBy = "Ryan Mitchell", ClosedAt = DateTime.UtcNow.AddDays(-2).AddHours(18), TenantId = DefaultTenantId },
            new WorkPermit { MineSiteId = site.Id, PermitNumber = "PTW-00002", Title = "Confined Space Entry - Fuel tank inspection", PermitType = "ConfinedSpace", RequestedBy = "Daniel Cooper", RequestDate = DateTime.UtcNow.AddDays(-1), StartDateTime = DateTime.UtcNow.AddHours(7), EndDateTime = DateTime.UtcNow.AddHours(15), Location = "ROM Stockpile - Fuel bay underground tank", WorkDescription = "Internal inspection of 50,000L underground diesel storage tank. Tank has been drained and ventilated for 48 hours.", HazardsIdentified = "Oxygen deficiency, residual vapours, confined space, falls", ControlMeasures = "Continuous gas monitoring, forced ventilation, standby person, rescue tripod with winch, 2-person buddy system", PPERequired = "Full body harness, gas monitor (4-gas), SCBA on standby, hard hat, torch", GasTestRequired = true, GasTestResults = "O2: 20.9%, LEL: 0%, CO: 0ppm, H2S: 0ppm - PASS", Status = "Active", ApprovedBy = "Craig Patterson", ApprovedAt = DateTime.UtcNow.AddDays(-1).AddHours(15), TenantId = DefaultTenantId },
            new WorkPermit { MineSiteId = site.Id, PermitNumber = "PTW-00003", Title = "Working at Heights - Conveyor belt replacement", PermitType = "WorkingAtHeights", RequestedBy = "Maintenance Planner", RequestDate = DateTime.UtcNow, StartDateTime = DateTime.UtcNow.AddDays(1).AddHours(6), EndDateTime = DateTime.UtcNow.AddDays(2).AddHours(18), Location = "Processing Plant - Transfer conveyor CV-04", WorkDescription = "Replacement of worn conveyor belt on CV-04 transfer conveyor. Work at 8m elevation. Crane lift required.", HazardsIdentified = "Falls from height, dropped objects, pinch points, crane operations", ControlMeasures = "Scaffold erected, full harness with dual lanyard, exclusion zone below, crane operator certified", PPERequired = "Full body harness, hard hat with chin strap, safety glasses, steel cap boots", GasTestRequired = false, Status = "Pending", TenantId = DefaultTenantId },
            new WorkPermit { MineSiteId = site.Id, PermitNumber = "PTW-00004", Title = "Electrical Isolation - Pump motor replacement", PermitType = "Electrical", RequestedBy = "Electrical Supervisor", RequestDate = DateTime.UtcNow.AddDays(-10), StartDateTime = DateTime.UtcNow.AddDays(-9).AddHours(7), EndDateTime = DateTime.UtcNow.AddDays(-9).AddHours(16), Location = "Pit dewatering station WNP-DS1", WorkDescription = "Isolation and replacement of 75kW submersible pump motor. Lock-out/tag-out procedure applies.", HazardsIdentified = "Electrical shock, stored energy, working near water, manual handling", ControlMeasures = "LOTO with personal padlocks, voltage testing before work, dry work area, mechanical lifting aids", PPERequired = "Electrical rated gloves (Class 0), safety glasses, hard hat, safety boots", GasTestRequired = false, Status = "Completed", ApprovedBy = "Tony Nguyen", ApprovedAt = DateTime.UtcNow.AddDays(-10).AddHours(14), ClosedBy = "Electrical Supervisor", ClosedAt = DateTime.UtcNow.AddDays(-9).AddHours(17), TenantId = DefaultTenantId },
        };
        context.WorkPermits.AddRange(permits);
        await context.SaveChangesAsync();

        return "Permits: Seeded 4 work permits.";
    }

    // ============================================
    // 11. Environmental
    // ============================================
    private static async Task<string> SeedEnvironmentalAsync(ApplicationDbContext context)
    {
        if (await context.EnvironmentalReadings.AnyAsync()) return "Environmental: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Environmental: No mine site found.";

        var readings = new List<EnvironmentalReading>();
        int readNum = 1;
        for (int day = 0; day < 7; day++)
        {
            var dt = DateTime.UtcNow.AddDays(-day).Date.AddHours(9);
            readings.Add(new EnvironmentalReading { MineSiteId = site.Id, ReadingNumber = $"ENV-{readNum++:D5}", ReadingType = "AirQuality", Parameter = "PM10", Value = day == 2 ? 82m : 35m + day * 5, Unit = "µg/m³", ThresholdMax = 70m, IsExceedance = day == 2, ReadingDateTime = dt, MonitoringStation = "Pit boundary - North", InstrumentUsed = "TEOM 1400ab", RecordedBy = "Rachel Kim", WeatherConditions = day == 2 ? "Strong NW wind 35km/h" : "Light breeze", Status = day == 2 ? "Exceedance" : "Normal", TenantId = DefaultTenantId });
            readings.Add(new EnvironmentalReading { MineSiteId = site.Id, ReadingNumber = $"ENV-{readNum++:D5}", ReadingType = "WaterQuality", Parameter = "pH", Value = 7.2m + day * 0.1m, Unit = "pH", ThresholdMin = 6.5m, ThresholdMax = 8.5m, IsExceedance = false, ReadingDateTime = dt, MonitoringStation = "TSF seepage bore MW-03", InstrumentUsed = "YSI ProDSS", RecordedBy = "Rachel Kim", Status = "Normal", TenantId = DefaultTenantId });
            readings.Add(new EnvironmentalReading { MineSiteId = site.Id, ReadingNumber = $"ENV-{readNum++:D5}", ReadingType = "Noise", Parameter = "LAeq (15min)", Value = 62m + day % 3 * 3, Unit = "dB(A)", ThresholdMax = 85m, IsExceedance = false, ReadingDateTime = dt, MonitoringStation = "Community receptor - Marradong Rd", InstrumentUsed = "Brüel & Kjær Type 2250", RecordedBy = "Rachel Kim", Status = "Normal", TenantId = DefaultTenantId });
        }
        context.EnvironmentalReadings.AddRange(readings);
        await context.SaveChangesAsync();

        var envIncidents = new[]
        {
            new EnvironmentalIncident { MineSiteId = site.Id, IncidentNumber = "EI-00001", Title = "PM10 exceedance at pit boundary monitoring station", IncidentType = "AirQuality", Severity = "Medium", OccurredAt = DateTime.UtcNow.AddDays(-2), Location = "Pit boundary - North monitoring station", Description = "24-hour average PM10 reading of 82 µg/m³ exceeded the licence limit of 70 µg/m³. Contributing factors include strong NW winds at 35 km/h and active loading operations in the pit.", ImpactAssessment = "Nearest sensitive receptor is 2.8km away. No community complaints received.", ContainmentActions = "Additional water carts deployed. Speed reduced on unsealed roads. Operations adjusted to minimise upwind activities.", ReportedBy = "Rachel Kim", NotifiedAuthority = true, AuthorityReference = "DMIRS Notification ref: ENV-2026-0341", Status = "Investigating", TenantId = DefaultTenantId },
        };
        context.EnvironmentalIncidents.AddRange(envIncidents);
        await context.SaveChangesAsync();

        return $"Environmental: Seeded {readings.Count} readings + 1 incident.";
    }

    // ============================================
    // 12. Ventilation & Gas
    // ============================================
    private static async Task<string> SeedVentilationAsync(ApplicationDbContext context)
    {
        if (await context.VentilationReadings.AnyAsync()) return "Ventilation: Already seeded.";
        var site2 = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "KSP-01");
        if (site2 == null) return "Ventilation: No underground mine site found.";
        var ugArea = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site2.Id && a.Code == "MCUG");

        var ventReadings = new List<VentilationReading>();
        var gasReadings = new List<GasReading>();
        int ventNum = 1, gasNum = 1;

        for (int day = 0; day < 7; day++)
        {
            var dt = DateTime.UtcNow.AddDays(-day).Date.AddHours(10);
            ventReadings.Add(new VentilationReading { MineSiteId = site2.Id, MineAreaId = ugArea?.Id, ReadingNumber = $"VNT-{ventNum++:D5}", LocationDescription = "Main decline - 650m level portal", AirflowVelocity = 3.2m + day * 0.1m, AirflowVolume = 48m + day * 1.5m, Temperature = 28.5m + day * 0.3m, Humidity = 72m + day * 2, BarometricPressure = 101.3m, ReadingDateTime = dt, RecordedBy = "Ventilation Officer", InstrumentUsed = "Kestrel 5500", DoorStatus = "Open", FanStatus = "Running", VentilationStatus = day == 5 ? "Warning" : "Normal", Notes = day == 5 ? "Elevated temperature - additional ventilation requested" : null, TenantId = DefaultTenantId });
            ventReadings.Add(new VentilationReading { MineSiteId = site2.Id, MineAreaId = ugArea?.Id, ReadingNumber = $"VNT-{ventNum++:D5}", LocationDescription = "Stope 14 - Level 8 drawpoint", AirflowVelocity = 1.8m + day * 0.05m, AirflowVolume = 22m + day * 0.8m, Temperature = 32m + day * 0.2m, Humidity = 85m, ReadingDateTime = dt, RecordedBy = "Ventilation Officer", VentilationStatus = "Normal", TenantId = DefaultTenantId });

            gasReadings.Add(new GasReading { MineSiteId = site2.Id, MineAreaId = ugArea?.Id, ReadingNumber = $"GAS-{gasNum++:D5}", GasType = "CO", Concentration = 5m + day * 0.5m, Unit = "ppm", ThresholdTWA = 30m, ThresholdSTEL = 100m, IsExceedance = false, LocationDescription = "Main decline - 650m level", ReadingDateTime = dt, RecordedBy = "Safety Officer", InstrumentId = "GD-001", CalibrationDate = DateTime.UtcNow.AddDays(-14), Status = "Normal", TenantId = DefaultTenantId });
            gasReadings.Add(new GasReading { MineSiteId = site2.Id, MineAreaId = ugArea?.Id, ReadingNumber = $"GAS-{gasNum++:D5}", GasType = "NO2", Concentration = day == 1 ? 4.5m : 1.2m + day * 0.2m, Unit = "ppm", ThresholdTWA = 3m, ThresholdSTEL = 5m, IsExceedance = day == 1, LocationDescription = "Post-blast re-entry check - Stope 14", ReadingDateTime = dt, RecordedBy = "Safety Officer", InstrumentId = "GD-001", ActionTaken = day == 1 ? "Area evacuated. Additional ventilation time required. Re-entry delayed 1 hour." : null, Status = day == 1 ? "Warning" : "Normal", TenantId = DefaultTenantId });
        }
        context.VentilationReadings.AddRange(ventReadings);
        context.GasReadings.AddRange(gasReadings);
        await context.SaveChangesAsync();

        return $"Ventilation: Seeded {ventReadings.Count} ventilation + {gasReadings.Count} gas readings.";
    }

    // ============================================
    // 13. Compliance
    // ============================================
    private static async Task<string> SeedComplianceAsync(ApplicationDbContext context)
    {
        if (await context.ComplianceRequirements.AnyAsync()) return "Compliance: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Compliance: No mine site found.";

        var req1Id = Guid.NewGuid();
        var req2Id = Guid.NewGuid();
        var requirements = new[]
        {
            new ComplianceRequirement { Id = req1Id, MineSiteId = site.Id, Code = "COMP-001", Title = "Annual Mine Closure Plan Update", Jurisdiction = "AU_WA", Category = "Mining", Description = "Submit updated Mine Closure Plan to DMIRS as per Mining Act 1978 and Mining Rehabilitation Fund Act 2012", RegulatoryBody = "DMIRS Western Australia", ReferenceDocument = "Mining Act 1978, s.70O", Frequency = "Annually", DueDate = DateTime.UtcNow.AddDays(60), LastCompletedDate = DateTime.UtcNow.AddDays(-300), NextDueDate = DateTime.UtcNow.AddDays(60), ResponsibleRole = "Mine Manager", Status = "Compliant", Priority = "High", PenaltyForNonCompliance = "Up to $50,000 fine and potential licence suspension", IsActive = true, TenantId = DefaultTenantId },
            new ComplianceRequirement { Id = req2Id, MineSiteId = site.Id, Code = "COMP-002", Title = "Quarterly Tailings Dam Stability Assessment", Jurisdiction = "AU_WA", Category = "Safety", Description = "Independent geotechnical assessment of TSF stability per ANCOLD guidelines and DMIRS Code of Practice", RegulatoryBody = "DMIRS Western Australia", ReferenceDocument = "DMIRS Code of Practice - Tailings", Frequency = "Quarterly", LastCompletedDate = DateTime.UtcNow.AddDays(-85), NextDueDate = DateTime.UtcNow.AddDays(-5), ResponsibleRole = "Geotechnical Engineer", Status = "Overdue", Priority = "Critical", PenaltyForNonCompliance = "Mandatory notification to DMIRS. Potential stop-work order.", IsActive = true, Notes = "Overdue by 5 days. Contractor availability issue.", TenantId = DefaultTenantId },
            new ComplianceRequirement { MineSiteId = site.Id, Code = "COMP-003", Title = "Monthly Safety Statistics Report", Jurisdiction = "AU_WA", Category = "Safety", Description = "Submit monthly safety performance report including LTIFR, TRIFR, and incident statistics", RegulatoryBody = "DMIRS Western Australia", Frequency = "Monthly", LastCompletedDate = DateTime.UtcNow.AddDays(-25), NextDueDate = DateTime.UtcNow.AddDays(5), ResponsibleRole = "Safety Manager", Status = "Compliant", Priority = "High", IsActive = true, TenantId = DefaultTenantId },
            new ComplianceRequirement { MineSiteId = site.Id, Code = "COMP-004", Title = "Annual Environmental Monitoring Report", Jurisdiction = "AU_WA", Category = "Environmental", Description = "Comprehensive annual report on all environmental monitoring parameters as per Environmental Licence conditions", RegulatoryBody = "DWER (Dept Water & Environmental Regulation)", ReferenceDocument = "Environmental Licence L8765/2014", Frequency = "Annually", LastCompletedDate = DateTime.UtcNow.AddDays(-200), NextDueDate = DateTime.UtcNow.AddDays(165), ResponsibleRole = "Environmental Officer", Status = "Compliant", Priority = "Medium", IsActive = true, TenantId = DefaultTenantId },
            new ComplianceRequirement { MineSiteId = site.Id, Code = "COMP-005", Title = "Dangerous Goods Licence Renewal", Jurisdiction = "AU_WA", Category = "Safety", Description = "Renew DG licence for explosive storage magazines", RegulatoryBody = "ChemCentre WA", ReferenceDocument = "Dangerous Goods Safety Act 2004", Frequency = "Annually", LastCompletedDate = DateTime.UtcNow.AddDays(-350), NextDueDate = DateTime.UtcNow.AddDays(15), ResponsibleRole = "Mine Manager", Status = "Compliant", Priority = "High", IsActive = true, TenantId = DefaultTenantId },
        };
        context.ComplianceRequirements.AddRange(requirements);
        await context.SaveChangesAsync();

        var audits = new[]
        {
            new ComplianceAudit { ComplianceRequirementId = req1Id, AuditNumber = "CA-00001", AuditDate = DateTime.UtcNow.AddDays(-300), AuditorName = "DMIRS Inspector - John Hardy", AuditType = "Regulatory", Findings = "Mine Closure Plan meets current requirements. Rehabilitation progress on track. Financial assurance adequate.", ComplianceStatus = "Compliant", Status = "Closed", TenantId = DefaultTenantId },
            new ComplianceAudit { ComplianceRequirementId = req2Id, AuditNumber = "CA-00002", AuditDate = DateTime.UtcNow.AddDays(-85), AuditorName = "GHD Consulting - Dr. Sarah Patel", AuditType = "External", Findings = "TSF operating within design parameters. Phreatic surface 2m below design level. Beach slope acceptable. Seepage monitoring bores within limits.", ComplianceStatus = "Compliant", Status = "Closed", TenantId = DefaultTenantId },
            new ComplianceAudit { ComplianceRequirementId = req2Id, AuditNumber = "CA-00003", AuditDate = DateTime.UtcNow.AddDays(-5), AuditorName = "Pending assignment", AuditType = "External", Findings = "Assessment overdue. Scheduling in progress.", ComplianceStatus = "NonCompliant", CorrectiveActions = "Expedite contractor engagement. Submit notification to DMIRS of delayed assessment.", ActionDueDate = DateTime.UtcNow.AddDays(7), Status = "Open", TenantId = DefaultTenantId },
        };
        context.ComplianceAudits.AddRange(audits);
        await context.SaveChangesAsync();

        return "Compliance: Seeded 5 requirements + 3 audits.";
    }

    // ============================================
    // 14. Geotechnical & Survey
    // ============================================
    private static async Task<string> SeedGeotechnicalAsync(ApplicationDbContext context)
    {
        if (await context.GeotechnicalAssessments.AnyAsync()) return "Geotechnical: Already seeded.";
        var site = await context.MineSites.FirstOrDefaultAsync(s => s.Code == "BGM-01");
        if (site == null) return "Geotechnical: No mine site found.";
        var pitArea = await context.MineAreas.FirstOrDefaultAsync(a => a.MineSiteId == site.Id && a.Code == "WNP");

        var assessments = new[]
        {
            new GeotechnicalAssessment { MineSiteId = site.Id, MineAreaId = pitArea?.Id, AssessmentNumber = "GEO-00001", Title = "WNP Bench 4 - Post rock-fall stability assessment", AssessmentType = "SlopeStability", Date = DateTime.UtcNow.AddDays(-42), AssessorName = "Dr. Alan Whitfield", Location = "Wandoo North Pit, Bench 4 western face", RockMassRating = 38m, SlopeAngle = 65m, WaterTableDepth = 12m, GroundCondition = "Laterite/dolerite contact, mod weathered", StabilityStatus = "Stable", RecommendedActions = "Install 4x monitoring prisms. Reduce face angle to 55° for next cut. 7-day scaling inspection interval.", MonitoringRequired = true, NextAssessmentDate = DateTime.UtcNow.AddDays(14), Status = "Completed", TenantId = DefaultTenantId },
            new GeotechnicalAssessment { MineSiteId = site.Id, MineAreaId = pitArea?.Id, AssessmentNumber = "GEO-00002", Title = "WNP Bench 7 - New tension crack assessment", AssessmentType = "SlopeStability", Date = DateTime.UtcNow.AddDays(-3), AssessorName = "Dr. Alan Whitfield", Location = "Wandoo North Pit, Bench 7 crest, 50m from western wall", RockMassRating = 45m, SlopeAngle = 70m, WaterTableDepth = 18m, GroundCondition = "Fresh dolerite, minor joint set", StabilityStatus = "MonitoringRequired", RecommendedActions = "Install crack monitoring pins. Daily visual inspection. If crack extends >12m, re-evaluate and consider unloading.", MonitoringRequired = true, NextAssessmentDate = DateTime.UtcNow.AddDays(7), Status = "Completed", TenantId = DefaultTenantId },
            new GeotechnicalAssessment { MineSiteId = site.Id, AssessmentNumber = "GEO-00003", Title = "TSF Dam 1 - Monthly embankment inspection", AssessmentType = "DamStability", Date = DateTime.UtcNow.AddDays(-15), AssessorName = "Dr. Alan Whitfield", Location = "TSF Dam 1 - main embankment", SlopeAngle = 18m, WaterTableDepth = 4.5m, GroundCondition = "Compacted earthfill, no seepage", StabilityStatus = "Stable", MonitoringRequired = true, NextAssessmentDate = DateTime.UtcNow.AddDays(15), Status = "Completed", TenantId = DefaultTenantId },
        };
        context.GeotechnicalAssessments.AddRange(assessments);
        await context.SaveChangesAsync();

        var surveys = new[]
        {
            new SurveyRecord { MineSiteId = site.Id, MineAreaId = pitArea?.Id, SurveyNumber = "SRV-00001", Title = "WNP Monthly Volume Survey - February 2026", SurveyType = "Volume", Date = DateTime.UtcNow.AddDays(-28), SurveyorName = "Greg Thompson", SurveyorLicense = "LS-WA-2019-0456", Location = "Wandoo North Pit - full pit survey", Easting = 432150m, Northing = 6375200m, Elevation = 285m, Datum = "GDA2020", CoordinateSystem = "MGA Zone 50", EquipmentUsed = "DJI Matrice 300 RTK + L1 LiDAR", Accuracy = "±50mm horizontal, ±100mm vertical", VolumeCalculated = 1850000m, AreaCalculated = 245000m, Findings = "Total material moved: 1.85M BCM. On track for annual plan.", Status = "Completed", TenantId = DefaultTenantId },
            new SurveyRecord { MineSiteId = site.Id, MineAreaId = pitArea?.Id, SurveyNumber = "SRV-00002", Title = "Bench 7 crack survey - detailed pick-up", SurveyType = "Structural", Date = DateTime.UtcNow.AddDays(-3), SurveyorName = "Greg Thompson", SurveyorLicense = "LS-WA-2019-0456", Location = "WNP Bench 7 crest - tension crack", Easting = 432280m, Northing = 6375350m, Elevation = 315m, Datum = "GDA2020", CoordinateSystem = "MGA Zone 50", EquipmentUsed = "Trimble S9 total station", Accuracy = "±2mm", Findings = "Crack trending 325°, 15cm max width, 8.2m mapped length. No vertical displacement.", Status = "Completed", TenantId = DefaultTenantId },
            new SurveyRecord { MineSiteId = site.Id, SurveyNumber = "SRV-00003", Title = "Waste Dump North - Monthly conformance survey", SurveyType = "Conformance", Date = DateTime.UtcNow.AddDays(3), SurveyorName = "Greg Thompson", Location = "Waste Dump North", Status = "Pending", TenantId = DefaultTenantId },
        };
        context.SurveyRecords.AddRange(surveys);
        await context.SaveChangesAsync();

        return "Geotechnical: Seeded 3 assessments + 3 surveys.";
    }
}
