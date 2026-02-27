using MediatR;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.CreateFeatureFlag;

public record CreateFeatureFlagCommand(
    string Name,
    string? Description,
    bool IsEnabled = false
) : IRequest<Guid>;
