using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Application.Features.MineSites.DTOs;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Queries.GetMineSiteById;

public record GetMineSiteByIdQuery(Guid Id) : IRequest<MineSiteDto>;

public class GetMineSiteByIdQueryHandler : IRequestHandler<GetMineSiteByIdQuery, MineSiteDto>
{
    private readonly IApplicationDbContext _context;

    public GetMineSiteByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MineSiteDto> Handle(GetMineSiteByIdQuery request, CancellationToken cancellationToken)
    {
        var mineSite = await _context.MineSites
            .AsNoTracking()
            .Where(m => m.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (mineSite is null)
            throw new NotFoundException(nameof(MineSite), request.Id);

        return mineSite;
    }
}
