using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Commands.CreateMineArea;

public class CreateMineAreaCommandHandler : IRequestHandler<CreateMineAreaCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateMineAreaCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateMineAreaCommand request, CancellationToken cancellationToken)
    {
        var mineSiteExists = await _context.MineSites
            .AnyAsync(m => m.Id == request.MineSiteId, cancellationToken);

        if (!mineSiteExists)
            throw new NotFoundException(nameof(MineSite), request.MineSiteId);

        if (request.ParentAreaId.HasValue)
        {
            var parentExists = await _context.MineAreas
                .AnyAsync(a => a.Id == request.ParentAreaId.Value && a.MineSiteId == request.MineSiteId, cancellationToken);

            if (!parentExists)
                throw new NotFoundException(nameof(MineArea), request.ParentAreaId.Value);
        }

        var area = new MineArea
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            Name = request.Name,
            Code = request.Code,
            AreaType = request.AreaType,
            Description = request.Description,
            Elevation = request.Elevation,
            IsActive = request.IsActive ?? true,
            ParentAreaId = request.ParentAreaId,
            SortOrder = request.SortOrder ?? 0,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.MineAreas.Add(area);
        await _context.SaveChangesAsync(cancellationToken);

        return area.Id;
    }
}
