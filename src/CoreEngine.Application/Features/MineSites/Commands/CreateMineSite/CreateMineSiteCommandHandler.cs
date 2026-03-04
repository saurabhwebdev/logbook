using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.MineSites.Commands.CreateMineSite;

public class CreateMineSiteCommandHandler : IRequestHandler<CreateMineSiteCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateMineSiteCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateMineSiteCommand request, CancellationToken cancellationToken)
    {
        var mineSite = new MineSite
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            MineType = request.MineType,
            Jurisdiction = request.Jurisdiction,
            JurisdictionDetails = request.JurisdictionDetails,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Address = request.Address,
            Country = request.Country,
            State = request.State,
            MineralsMined = request.MineralsMined,
            OperatingCompany = request.OperatingCompany,
            MiningLicenseNumber = request.MiningLicenseNumber,
            LicenseExpiryDate = request.LicenseExpiryDate,
            OperationalSince = request.OperationalSince,
            Status = request.Status ?? "Active",
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            NearestHospital = request.NearestHospital,
            NearestHospitalPhone = request.NearestHospitalPhone,
            NearestHospitalDistanceKm = request.NearestHospitalDistanceKm,
            UnitSystem = request.UnitSystem ?? "Metric",
            TimeZone = request.TimeZone ?? "UTC",
            ShiftsPerDay = request.ShiftsPerDay ?? 3,
            ShiftPattern = request.ShiftPattern,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.MineSites.Add(mineSite);
        await _context.SaveChangesAsync(cancellationToken);

        return mineSite.Id;
    }
}
