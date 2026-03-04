using MediatR;

namespace CoreEngine.Application.Features.MineSites.Commands.UpdateMineArea;

public record UpdateMineAreaCommand(
    Guid Id,
    string Name,
    string? Code,
    string AreaType,
    string? Description,
    double? Elevation,
    bool IsActive,
    Guid? ParentAreaId,
    int SortOrder
) : IRequest<Unit>;
