using MediatR;

namespace CoreEngine.Application.Features.MineSites.Commands.CreateMineSite;

public record CreateMineSiteCommand(
    string Name,
    string? Code,
    string MineType,
    string Jurisdiction,
    string? JurisdictionDetails,
    double? Latitude,
    double? Longitude,
    string? Address,
    string? Country,
    string? State,
    string? MineralsMined,
    string? OperatingCompany,
    string? MiningLicenseNumber,
    DateTime? LicenseExpiryDate,
    DateTime? OperationalSince,
    string? Status,
    string? EmergencyContactName,
    string? EmergencyContactPhone,
    string? NearestHospital,
    string? NearestHospitalPhone,
    double? NearestHospitalDistanceKm,
    string? UnitSystem,
    string? TimeZone,
    int? ShiftsPerDay,
    string? ShiftPattern
) : IRequest<Guid>;
