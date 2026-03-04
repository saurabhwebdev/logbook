using MediatR;

namespace CoreEngine.Application.Features.MineSites.Commands.DeleteMineArea;

public record DeleteMineAreaCommand(Guid Id) : IRequest<Unit>;
