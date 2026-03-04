using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Production.Commands.CreateProductionLog;

public class CreateProductionLogCommandHandler : IRequestHandler<CreateProductionLogCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateProductionLogCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateProductionLogCommand request, CancellationToken cancellationToken)
    {
        var count = await _context.ProductionLogs.CountAsync(cancellationToken) + 1;
        var logNumber = $"PRD-{count:D5}";

        var entity = new ProductionLog
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            MineAreaId = request.MineAreaId,
            ShiftInstanceId = request.ShiftInstanceId,
            LogNumber = logNumber,
            Date = request.Date,
            ShiftName = request.ShiftName,
            Material = request.Material,
            SourceLocation = request.SourceLocation,
            DestinationLocation = request.DestinationLocation,
            QuantityTonnes = request.QuantityTonnes,
            QuantityBCM = request.QuantityBCM,
            EquipmentUsed = request.EquipmentUsed,
            OperatorName = request.OperatorName,
            HaulingDistance = request.HaulingDistance,
            LoadCount = request.LoadCount,
            Status = "Draft",
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductionLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
