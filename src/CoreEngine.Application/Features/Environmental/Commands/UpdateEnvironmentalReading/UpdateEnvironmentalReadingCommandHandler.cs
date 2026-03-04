using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Environmental.Commands.UpdateEnvironmentalReading;

public class UpdateEnvironmentalReadingCommandHandler : IRequestHandler<UpdateEnvironmentalReadingCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateEnvironmentalReadingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEnvironmentalReadingCommand request, CancellationToken cancellationToken)
    {
        var reading = await _context.EnvironmentalReadings
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Environmental reading {request.Id} not found.");

        reading.ReadingType = request.ReadingType;
        reading.Parameter = request.Parameter;
        reading.Value = request.Value;
        reading.Unit = request.Unit;
        reading.ThresholdMin = request.ThresholdMin;
        reading.ThresholdMax = request.ThresholdMax;
        reading.IsExceedance = request.IsExceedance;
        reading.ReadingDateTime = request.ReadingDateTime;
        reading.MonitoringStation = request.MonitoringStation;
        reading.InstrumentUsed = request.InstrumentUsed;
        reading.CalibratedDate = request.CalibratedDate;
        reading.RecordedBy = request.RecordedBy;
        reading.WeatherConditions = request.WeatherConditions;
        reading.Notes = request.Notes;
        reading.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
