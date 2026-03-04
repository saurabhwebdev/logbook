using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.DeleteStatutoryRegister;

public class DeleteStatutoryRegisterCommandHandler : IRequestHandler<DeleteStatutoryRegisterCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteStatutoryRegisterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteStatutoryRegisterCommand request, CancellationToken cancellationToken)
    {
        var register = await _context.StatutoryRegisters
            .Include(s => s.Entries)
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (register is null)
            throw new NotFoundException(nameof(StatutoryRegister), request.Id);

        if (register.Entries.Any())
            throw new ConflictException("Cannot delete statutory register with existing entries. Registers with entries must be retained for compliance.");

        register.IsDeleted = true;
        register.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
