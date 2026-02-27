using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.UpdateFeatureFlag;

public class UpdateFeatureFlagCommandHandler : IRequestHandler<UpdateFeatureFlagCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateFeatureFlagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = await _context.FeatureFlags
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (featureFlag is null)
            throw new NotFoundException("FeatureFlag", request.Id);

        featureFlag.Name = request.Name;
        featureFlag.Description = request.Description;
        featureFlag.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
