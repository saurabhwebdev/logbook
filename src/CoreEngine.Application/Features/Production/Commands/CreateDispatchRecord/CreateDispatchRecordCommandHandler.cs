using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Commands.CreateDispatchRecord;

public class CreateDispatchRecordCommandHandler : IRequestHandler<CreateDispatchRecordCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateDispatchRecordCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateDispatchRecordCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.DispatchRecords.CountAsync(cancellationToken) + 1;
        var dispatchNumber = $"DSP-{count:D5}";

        var entity = new DispatchRecord
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            DispatchNumber = dispatchNumber,
            Date = request.Date,
            VehicleNumber = request.VehicleNumber,
            DriverName = request.DriverName,
            Material = request.Material,
            SourceLocation = request.SourceLocation,
            DestinationLocation = request.DestinationLocation,
            WeighbridgeTicketNumber = request.WeighbridgeTicketNumber,
            GrossWeight = request.GrossWeight,
            TareWeight = request.TareWeight,
            NetWeight = request.NetWeight,
            Unit = request.Unit ?? "Tonnes",
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime,
            Status = "Loading",
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.DispatchRecords.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
