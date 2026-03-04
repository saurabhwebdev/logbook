using MediatR;

namespace CoreEngine.Application.Features.Compliance.Commands.UpdateComplianceRequirement;

public record UpdateComplianceRequirementCommand(
    Guid Id,
    string Title,
    string Jurisdiction,
    string Category,
    string Description,
    string? RegulatoryBody,
    string? ReferenceDocument,
    string Frequency,
    DateTime? DueDate,
    DateTime? LastCompletedDate,
    DateTime? NextDueDate,
    string? ResponsibleRole,
    string Status,
    string Priority,
    string? PenaltyForNonCompliance,
    string? Notes,
    bool IsActive
) : IRequest;
