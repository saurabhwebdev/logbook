using MediatR;

namespace CoreEngine.Application.Features.Compliance.Commands.CreateComplianceRequirement;

public record CreateComplianceRequirementCommand(
    Guid MineSiteId,
    string Code,
    string Title,
    string Jurisdiction,
    string Category,
    string Description,
    string? RegulatoryBody,
    string? ReferenceDocument,
    string Frequency,
    DateTime? DueDate,
    DateTime? NextDueDate,
    string? ResponsibleRole,
    string Priority,
    string? PenaltyForNonCompliance,
    string? Notes
) : IRequest<Guid>;
