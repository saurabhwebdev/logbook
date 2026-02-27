using MediatR;

namespace CoreEngine.Application.Features.Emails.Commands.QueueEmail;

public record QueueEmailCommand(
    string To,
    string Subject,
    string HtmlBody,
    string? PlainTextBody
) : IRequest<Unit>;
