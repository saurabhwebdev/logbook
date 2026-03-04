using MediatR;

namespace CoreEngine.Application.Features.MineSites.Commands.DeleteMineSite;

public record DeleteMineSiteCommand(Guid Id) : IRequest<Unit>;
