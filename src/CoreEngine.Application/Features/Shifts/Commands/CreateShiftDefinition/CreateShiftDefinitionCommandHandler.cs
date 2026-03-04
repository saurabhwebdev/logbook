using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.CreateShiftDefinition;

public class CreateShiftDefinitionCommandHandler : IRequestHandler<CreateShiftDefinitionCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateShiftDefinitionCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateShiftDefinitionCommand request, CancellationToken cancellationToken)
    {
        var mineSiteExists = await _context.MineSites
            .AnyAsync(m => m.Id == request.MineSiteId, cancellationToken);

        if (!mineSiteExists)
            throw new NotFoundException(nameof(MineSite), request.MineSiteId);

        var entity = new ShiftDefinition
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            Name = request.Name,
            Code = request.Code,
            StartTime = TimeSpan.Parse(request.StartTime),
            EndTime = TimeSpan.Parse(request.EndTime),
            ShiftOrder = request.ShiftOrder ?? 0,
            Color = request.Color,
            IsActive = request.IsActive ?? true,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ShiftDefinitions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
