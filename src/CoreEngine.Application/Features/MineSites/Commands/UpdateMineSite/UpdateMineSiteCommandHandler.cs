using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Commands.UpdateMineSite;

public class UpdateMineSiteCommandHandler : IRequestHandler<UpdateMineSiteCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateMineSiteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateMineSiteCommand request, CancellationToken cancellationToken)
    {
        var mineSite = await _context.MineSites
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (mineSite is null)
            throw new NotFoundException(nameof(MineSite), request.Id);

        mineSite.Name = request.Name;
        mineSite.Code = request.Code;
        mineSite.MineType = request.MineType;
        mineSite.Jurisdiction = request.Jurisdiction;
        mineSite.JurisdictionDetails = request.JurisdictionDetails;
        mineSite.Latitude = request.Latitude;
        mineSite.Longitude = request.Longitude;
        mineSite.Address = request.Address;
        mineSite.Country = request.Country;
        mineSite.State = request.State;
        mineSite.MineralsMined = request.MineralsMined;
        mineSite.OperatingCompany = request.OperatingCompany;
        mineSite.MiningLicenseNumber = request.MiningLicenseNumber;
        mineSite.LicenseExpiryDate = request.LicenseExpiryDate;
        mineSite.OperationalSince = request.OperationalSince;
        mineSite.Status = request.Status;
        mineSite.EmergencyContactName = request.EmergencyContactName;
        mineSite.EmergencyContactPhone = request.EmergencyContactPhone;
        mineSite.NearestHospital = request.NearestHospital;
        mineSite.NearestHospitalPhone = request.NearestHospitalPhone;
        mineSite.NearestHospitalDistanceKm = request.NearestHospitalDistanceKm;
        mineSite.UnitSystem = request.UnitSystem;
        mineSite.TimeZone = request.TimeZone;
        mineSite.ShiftsPerDay = request.ShiftsPerDay;
        mineSite.ShiftPattern = request.ShiftPattern;
        mineSite.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
