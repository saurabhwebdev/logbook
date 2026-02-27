using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.ToggleFeatureFlag;

public record ToggleFeatureFlagCommand(Guid Id, bool IsEnabled) : IRequest;

public class ToggleFeatureFlagCommandHandler : IRequestHandler<ToggleFeatureFlagCommand>
{
    private readonly IApplicationDbContext _context;
    public ToggleFeatureFlagCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(ToggleFeatureFlagCommand request, CancellationToken ct)
    {
        var flag = await _context.FeatureFlags.FindAsync(new object[] { request.Id }, ct)
            ?? throw new NotFoundException("FeatureFlag", request.Id);
        flag.IsEnabled = request.IsEnabled;
        await _context.SaveChangesAsync(ct);
    }
}
