using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Blasting.Commands.CreateExplosiveUsage;

public record CreateExplosiveUsageCommand(
    Guid BlastEventId,
    string ExplosiveName,
    string Type,
    string? BatchNumber,
    decimal QuantityIssued,
    decimal QuantityUsed,
    decimal QuantityReturned,
    string Unit,
    string? MagazineSource,
    string? IssuedBy,
    string? ReceivedBy,
    string? Notes
) : IRequest<Guid>;

public class CreateExplosiveUsageCommandHandler : IRequestHandler<CreateExplosiveUsageCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateExplosiveUsageCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateExplosiveUsageCommand request, CancellationToken cancellationToken)
    {
        var usage = new ExplosiveUsage
        {
            Id = Guid.NewGuid(),
            BlastEventId = request.BlastEventId,
            ExplosiveName = request.ExplosiveName,
            Type = request.Type,
            BatchNumber = request.BatchNumber,
            QuantityIssued = request.QuantityIssued,
            QuantityUsed = request.QuantityUsed,
            QuantityReturned = request.QuantityReturned,
            Unit = request.Unit,
            MagazineSource = request.MagazineSource,
            IssuedBy = request.IssuedBy,
            ReceivedBy = request.ReceivedBy,
            Notes = request.Notes,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.ExplosiveUsages.Add(usage);
        await _context.SaveChangesAsync(cancellationToken);

        return usage.Id;
    }
}
