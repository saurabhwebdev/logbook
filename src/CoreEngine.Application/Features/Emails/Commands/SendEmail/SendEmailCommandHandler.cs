using CoreEngine.Application.Common.Interfaces;
using MediatR;

namespace CoreEngine.Application.Features.Emails.Commands.SendEmail;

public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, bool>
{
    private readonly IEmailService _emailService;
    public SendEmailCommandHandler(IEmailService emailService) => _emailService = emailService;

    public async Task<bool> Handle(SendEmailCommand request, CancellationToken ct)
    {
        return await _emailService.SendEmailAsync(request.To, request.Subject, request.HtmlBody, request.PlainTextBody, ct);
    }
}
