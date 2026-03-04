using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Commands.UpdateProductionLog;

public class UpdateProductionLogCommandHandler : IRequestHandler<UpdateProductionLogCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateProductionLogCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateProductionLogCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ProductionLogs
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Production log {request.Id} not found.");

        entity.MineAreaId = request.MineAreaId;
        entity.ShiftInstanceId = request.ShiftInstanceId;
        entity.Date = request.Date;
        entity.ShiftName = request.ShiftName;
        entity.Material = request.Material;
        entity.SourceLocation = request.SourceLocation;
        entity.DestinationLocation = request.DestinationLocation;
        entity.QuantityTonnes = request.QuantityTonnes;
        entity.QuantityBCM = request.QuantityBCM;
        entity.EquipmentUsed = request.EquipmentUsed;
        entity.OperatorName = request.OperatorName;
        entity.HaulingDistance = request.HaulingDistance;
        entity.LoadCount = request.LoadCount;
        entity.Status = request.Status;
        entity.Notes = request.Notes;
        entity.VerifiedBy = request.VerifiedBy;
        entity.VerifiedAt = request.VerifiedAt;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
