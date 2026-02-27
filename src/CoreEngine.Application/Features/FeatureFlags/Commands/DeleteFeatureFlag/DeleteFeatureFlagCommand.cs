using MediatR;

namespace CoreEngine.Application.Features.FeatureFlags.Commands.DeleteFeatureFlag;

public record DeleteFeatureFlagCommand(Guid Id) : IRequest<Unit>;
