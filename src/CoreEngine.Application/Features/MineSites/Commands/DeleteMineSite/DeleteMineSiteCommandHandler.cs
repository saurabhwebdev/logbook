using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.MineSites.Commands.DeleteMineSite;

public class DeleteMineSiteCommandHandler : IRequestHandler<DeleteMineSiteCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteMineSiteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteMineSiteCommand request, CancellationToken cancellationToken)
    {
        var mineSite = await _context.MineSites
            .Include(m => m.MineAreas)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (mineSite is null)
            throw new NotFoundException(nameof(MineSite), request.Id);

        if (mineSite.MineAreas.Any())
            throw new ConflictException("Cannot delete mine site with existing areas. Remove all areas first.");

        mineSite.IsDeleted = true;
        mineSite.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
