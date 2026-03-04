using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.MineSites.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Queries.GetMineSites;

public record GetMineSitesQuery() : IRequest<IReadOnlyList<MineSiteDto>>;

public class GetMineSitesQueryHandler : IRequestHandler<GetMineSitesQuery, IReadOnlyList<MineSiteDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMineSitesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<MineSiteDto>> Handle(GetMineSitesQuery request, CancellationToken cancellationToken)
    {
        var mineSites = await _context.MineSites
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Select(m => new MineSiteDto(
                m.Id,
                m.Name,
                m.Code,
                m.MineType,
                m.Jurisdiction,
                m.JurisdictionDetails,
                m.Latitude,
                m.Longitude,
                m.Address,
                m.Country,
                m.State,
                m.MineralsMined,
                m.OperatingCompany,
                m.MiningLicenseNumber,
                m.LicenseExpiryDate,
                m.OperationalSince,
                m.Status,
                m.EmergencyContactName,
                m.EmergencyContactPhone,
                m.NearestHospital,
                m.NearestHospitalPhone,
                m.NearestHospitalDistanceKm,
                m.UnitSystem,
                m.TimeZone,
                m.ShiftsPerDay,
                m.ShiftPattern,
                m.CreatedAt,
                m.MineAreas.Count
            ))
            .ToListAsync(cancellationToken);

        return mineSites;
    }
}
