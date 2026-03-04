using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Ventilation.Commands.UpdateVentilationReading;

public class UpdateVentilationReadingCommandHandler : IRequestHandler<UpdateVentilationReadingCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateVentilationReadingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateVentilationReadingCommand request, CancellationToken cancellationToken)
    {
        var reading = await _context.VentilationReadings
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Ventilation reading {request.Id} not found.");

        reading.LocationDescription = request.LocationDescription;
        reading.AirflowVelocity = request.AirflowVelocity;
        reading.AirflowVolume = request.AirflowVolume;
        reading.Temperature = request.Temperature;
        reading.Humidity = request.Humidity;
        reading.BarometricPressure = request.BarometricPressure;
        reading.ReadingDateTime = request.ReadingDateTime;
        reading.RecordedBy = request.RecordedBy;
        reading.InstrumentUsed = request.InstrumentUsed;
        reading.DoorStatus = request.DoorStatus;
        reading.FanStatus = request.FanStatus;
        reading.VentilationStatus = request.VentilationStatus;
        reading.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
