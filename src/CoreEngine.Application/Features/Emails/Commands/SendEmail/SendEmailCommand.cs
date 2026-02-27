using MediatR;

namespace CoreEngine.Application.Features.Emails.Commands.SendEmail;

public record SendEmailCommand(
    string To,
    string Subject,
    string HtmlBody,
    string? PlainTextBody
) : IRequest<bool>;
