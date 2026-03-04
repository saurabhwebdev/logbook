using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Equipment.Commands.DeleteEquipment;

public record DeleteEquipmentCommand(Guid Id) : IRequest;

public class DeleteEquipmentCommandHandler : IRequestHandler<DeleteEquipmentCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteEquipmentCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteEquipmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Equipment
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Equipment {request.Id} not found.");

        _context.Equipment.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
