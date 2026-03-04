using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Commands.CreateEnvironmentalReading;

public class CreateEnvironmentalReadingCommandHandler : IRequestHandler<CreateEnvironmentalReadingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateEnvironmentalReadingCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateEnvironmentalReadingCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.EnvironmentalReadings.CountAsync(cancellationToken) + 1;
        var readingNumber = $"ENV-{count:D5}";

        var status = "Normal";
        if (request.IsExceedance)
            status = "Exceedance";
        else if (request.ThresholdMax.HasValue && request.Value > request.ThresholdMax.Value * 0.9m)
            status = "Warning";
        else if (request.ThresholdMin.HasValue && request.Value < request.ThresholdMin.Value * 1.1m)
            status = "Warning";

        var reading = new EnvironmentalReading
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            ReadingNumber = readingNumber,
            ReadingType = request.ReadingType,
            Parameter = request.Parameter,
            Value = request.Value,
            Unit = request.Unit,
            ThresholdMin = request.ThresholdMin,
            ThresholdMax = request.ThresholdMax,
            IsExceedance = request.IsExceedance,
            ReadingDateTime = request.ReadingDateTime,
            MonitoringStation = request.MonitoringStation,
            InstrumentUsed = request.InstrumentUsed,
            CalibratedDate = request.CalibratedDate,
            RecordedBy = request.RecordedBy,
            WeatherConditions = request.WeatherConditions,
            Notes = request.Notes,
            Status = status,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.EnvironmentalReadings.Add(reading);
        await _context.SaveChangesAsync(cancellationToken);

        return reading.Id;
    }
}
