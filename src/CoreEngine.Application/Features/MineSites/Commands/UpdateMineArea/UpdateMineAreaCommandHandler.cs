using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Commands.UpdateMineArea;

public class UpdateMineAreaCommandHandler : IRequestHandler<UpdateMineAreaCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateMineAreaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateMineAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.MineAreas
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (area is null)
            throw new NotFoundException(nameof(MineArea), request.Id);

        if (request.ParentAreaId.HasValue && request.ParentAreaId.Value == request.Id)
            throw new ConflictException("An area cannot be its own parent.");

        area.Name = request.Name;
        area.Code = request.Code;
        area.AreaType = request.AreaType;
        area.Description = request.Description;
        area.Elevation = request.Elevation;
        area.IsActive = request.IsActive;
        area.ParentAreaId = request.ParentAreaId;
        area.SortOrder = request.SortOrder;
        area.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
