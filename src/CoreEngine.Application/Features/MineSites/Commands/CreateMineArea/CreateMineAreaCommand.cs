using MediatR;

namespace CoreEngine.Application.Features.MineSites.Commands.CreateMineArea;

public record CreateMineAreaCommand(
    Guid MineSiteId,
    string Name,
    string? Code,
    string AreaType,
    string? Description,
    double? Elevation,
    bool? IsActive,
    Guid? ParentAreaId,
    int? SortOrder
) : IRequest<Guid>;
