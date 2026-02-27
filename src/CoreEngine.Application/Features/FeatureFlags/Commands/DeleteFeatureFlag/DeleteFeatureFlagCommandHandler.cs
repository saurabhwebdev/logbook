using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.DeleteFeatureFlag;

public class DeleteFeatureFlagCommandHandler : IRequestHandler<DeleteFeatureFlagCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteFeatureFlagCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteFeatureFlagCommand request, CancellationToken cancellationToken)
    {
        var featureFlag = await _context.FeatureFlags.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException("FeatureFlag", request.Id);

        _context.FeatureFlags.Remove(featureFlag);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
