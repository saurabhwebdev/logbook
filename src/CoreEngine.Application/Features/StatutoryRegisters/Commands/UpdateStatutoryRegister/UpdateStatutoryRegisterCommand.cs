using MediatR;

namespace CoreEngine.Application.Features.StatutoryRegisters.Commands.UpdateStatutoryRegister;

public record UpdateStatutoryRegisterCommand(
    Guid Id,
    string Name,
    string? Code,
    string RegisterType,
    string? Description,
    string Jurisdiction,
    bool IsRequired,
    int RetentionYears,
    bool IsActive,
    int SortOrder
) : IRequest<Unit>;
