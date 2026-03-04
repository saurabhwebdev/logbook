using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Ventilation.Commands.CreateVentilationReading;

public class CreateVentilationReadingCommandHandler : IRequestHandler<CreateVentilationReadingCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateVentilationReadingCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateVentilationReadingCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.VentilationReadings.CountAsync(cancellationToken) + 1;
        var readingNumber = $"VNT-{count:D5}";

        var status = "Normal";
        if (request.AirflowVelocity.HasValue && request.AirflowVelocity.Value <= 0)
            status = "Critical";
        else if (request.AirflowVolume.HasValue && request.AirflowVolume.Value <= 0)
            status = "Critical";

        var reading = new VentilationReading
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            ReadingNumber = readingNumber,
            LocationDescription = request.LocationDescription,
            AirflowVelocity = request.AirflowVelocity,
            AirflowVolume = request.AirflowVolume,
            Temperature = request.Temperature,
            Humidity = request.Humidity,
            BarometricPressure = request.BarometricPressure,
            ReadingDateTime = request.ReadingDateTime,
            RecordedBy = request.RecordedBy,
            InstrumentUsed = request.InstrumentUsed,
            DoorStatus = request.DoorStatus,
            FanStatus = request.FanStatus,
            VentilationStatus = status,
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.VentilationReadings.Add(reading);
        await _context.SaveChangesAsync(cancellationToken);

        return reading.Id;
    }
}
