using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Shifts.Commands.UpdateShiftDefinition;

public class UpdateShiftDefinitionCommandHandler : IRequestHandler<UpdateShiftDefinitionCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateShiftDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateShiftDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.ShiftDefinitions
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(ShiftDefinition), request.Id);

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.StartTime = TimeSpan.Parse(request.StartTime);
        entity.EndTime = TimeSpan.Parse(request.EndTime);
        entity.ShiftOrder = request.ShiftOrder;
        entity.Color = request.Color;
        entity.IsActive = request.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
