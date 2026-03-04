using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Commands.UpdateDispatchRecord;

public class UpdateDispatchRecordCommandHandler : IRequestHandler<UpdateDispatchRecordCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateDispatchRecordCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateDispatchRecordCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.DispatchRecords
            .FirstOrDefaultAsync(d => d.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Dispatch record {request.Id} not found.");

        entity.Date = request.Date;
        entity.VehicleNumber = request.VehicleNumber;
        entity.DriverName = request.DriverName;
        entity.Material = request.Material;
        entity.SourceLocation = request.SourceLocation;
        entity.DestinationLocation = request.DestinationLocation;
        entity.WeighbridgeTicketNumber = request.WeighbridgeTicketNumber;
        entity.GrossWeight = request.GrossWeight;
        entity.TareWeight = request.TareWeight;
        entity.NetWeight = request.NetWeight;
        entity.Unit = request.Unit;
        entity.DepartureTime = request.DepartureTime;
        entity.ArrivalTime = request.ArrivalTime;
        entity.Status = request.Status;
        entity.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
