using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.CreateFeatureFlag;

public class CreateFeatureFlagCommandHandler : IRequestHandler<CreateFeatureFlagCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateFeatureFlagCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = new FeatureFlag
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            IsEnabled = request.IsEnabled,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.FeatureFlags.Add(featureFlag);
        await _context.SaveChangesAsync(cancellationToken);

        return featureFlag.Id;
    }
}
