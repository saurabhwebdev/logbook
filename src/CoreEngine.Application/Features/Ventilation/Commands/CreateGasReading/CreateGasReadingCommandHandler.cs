using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Ventilation.Commands.CreateGasReading;

public class CreateGasReadingCommandHandler : IRequestHandler<CreateGasReadingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateGasReadingCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateGasReadingCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.GasReadings.CountAsync(cancellationToken) + 1;
        var readingNumber = $"GAS-{count:D5}";

        var status = "Normal";
        if (request.IsExceedance)
            status = "Alarm";
        else if (request.ThresholdSTEL.HasValue && request.Concentration > request.ThresholdSTEL.Value * 0.8m)
            status = "Warning";
        else if (request.ThresholdTWA.HasValue && request.Concentration > request.ThresholdTWA.Value * 0.9m)
            status = "Warning";

        var reading = new GasReading
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            ReadingNumber = readingNumber,
            GasType = request.GasType,
            Concentration = request.Concentration,
            Unit = request.Unit,
            ThresholdTWA = request.ThresholdTWA,
            ThresholdSTEL = request.ThresholdSTEL,
            ThresholdCeiling = request.ThresholdCeiling,
            IsExceedance = request.IsExceedance,
            LocationDescription = request.LocationDescription,
            ReadingDateTime = request.ReadingDateTime,
            RecordedBy = request.RecordedBy,
            InstrumentId = request.InstrumentId,
            CalibrationDate = request.CalibrationDate,
            ActionTaken = request.ActionTaken,
            Status = status,
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.GasReadings.Add(reading);
        await _context.SaveChangesAsync(cancellationToken);

        return reading.Id;
    }
}
