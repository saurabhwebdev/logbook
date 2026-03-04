using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.UpdateStatutoryRegister;

public class UpdateStatutoryRegisterCommandHandler : IRequestHandler<UpdateStatutoryRegisterCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateStatutoryRegisterCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateStatutoryRegisterCommand request, CancellationToken cancellationToken)
    {
        var register = await _context.StatutoryRegisters
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (register is null)
            throw new NotFoundException(nameof(StatutoryRegister), request.Id);

        register.Name = request.Name;
        register.Code = request.Code;
        register.RegisterType = request.RegisterType;
        register.Description = request.Description;
        register.Jurisdiction = request.Jurisdiction;
        register.IsRequired = request.IsRequired;
        register.RetentionYears = request.RetentionYears;
        register.IsActive = request.IsActive;
        register.SortOrder = request.SortOrder;
        register.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
