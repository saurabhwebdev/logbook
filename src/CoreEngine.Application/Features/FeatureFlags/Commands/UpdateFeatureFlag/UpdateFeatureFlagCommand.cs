using MediatR;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.UpdateFeatureFlag;

public record UpdateFeatureFlagCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest<Unit>;
