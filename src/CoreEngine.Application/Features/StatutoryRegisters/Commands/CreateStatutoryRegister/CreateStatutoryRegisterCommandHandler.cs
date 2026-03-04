using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateStatutoryRegister;

public class CreateStatutoryRegisterCommandHandler : IRequestHandler<CreateStatutoryRegisterCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateStatutoryRegisterCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateStatutoryRegisterCommand request, CancellationToken cancellationToken)
    {
        var register = new StatutoryRegister
        {
            Id = Guid.NewGuid(),
            MineSiteId = request.MineSiteId,
            Name = request.Name,
            Code = request.Code,
            RegisterType = request.RegisterType,
            Description = request.Description,
            Jurisdiction = request.Jurisdiction,
            IsRequired = request.IsRequired ?? true,
            RetentionYears = request.RetentionYears ?? 5,
            IsActive = request.IsActive ?? true,
            SortOrder = request.SortOrder ?? 0,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StatutoryRegisters.Add(register);
        await _context.SaveChangesAsync(cancellationToken);

        return register.Id;
    }
}
