using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Ventilation.Commands.UpdateGasReading;

public class UpdateGasReadingCommandHandler : IRequestHandler<UpdateGasReadingCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateGasReadingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateGasReadingCommand request, CancellationToken cancellationToken)
    {
        var reading = await _context.GasReadings
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Gas reading {request.Id} not found.");

        reading.GasType = request.GasType;
        reading.Concentration = request.Concentration;
        reading.Unit = request.Unit;
        reading.ThresholdTWA = request.ThresholdTWA;
        reading.ThresholdSTEL = request.ThresholdSTEL;
        reading.ThresholdCeiling = request.ThresholdCeiling;
        reading.IsExceedance = request.IsExceedance;
        reading.LocationDescription = request.LocationDescription;
        reading.ReadingDateTime = request.ReadingDateTime;
        reading.RecordedBy = request.RecordedBy;
        reading.InstrumentId = request.InstrumentId;
        reading.CalibrationDate = request.CalibrationDate;
        reading.ActionTaken = request.ActionTaken;
        reading.Status = request.Status;
        reading.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
