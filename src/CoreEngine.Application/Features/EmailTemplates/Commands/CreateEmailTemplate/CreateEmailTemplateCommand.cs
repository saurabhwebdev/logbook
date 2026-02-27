using MediatR;

namespace CoreEngine.Application.Features.EmailTemplates.Commands.CreateEmailTemplate;

public record CreateEmailTemplateCommand(
    string Name,
    string Subject,
    string HtmlBody,
    string? PlainTextBody,
    bool IsActive
) : IRequest<Guid>;
