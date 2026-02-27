using MediatR;

namespace CoreEngine.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate;

public record UpdateEmailTemplateCommand(
    Guid Id,
    string Name,
    string Subject,
    string HtmlBody,
    string? PlainTextBody,
    bool IsActive
) : IRequest<Unit>;
