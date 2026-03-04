using MediatR;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.CreateStatutoryRegister;

public record CreateStatutoryRegisterCommand(
    Guid MineSiteId,
    string Name,
    string? Code,
    string RegisterType,
    string? Description,
    string Jurisdiction,
    bool? IsRequired,
    int? RetentionYears,
    bool? IsActive,
    int? SortOrder
) : IRequest<Guid>;
