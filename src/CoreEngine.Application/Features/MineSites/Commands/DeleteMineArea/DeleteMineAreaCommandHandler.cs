using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Commands.DeleteMineArea;

public class DeleteMineAreaCommandHandler : IRequestHandler<DeleteMineAreaCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteMineAreaCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteMineAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.MineAreas
            .Include(a => a.ChildAreas)
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (area is null)
            throw new NotFoundException(nameof(MineArea), request.Id);

        if (area.ChildAreas.Any())
            throw new ConflictException("Cannot delete area with child areas. Remove child areas first.");

        area.IsDeleted = true;
        area.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
