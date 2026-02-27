using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CoreEngine.Application.Features.Emails.Commands.ProcessEmailQueue;

public record ProcessEmailQueueCommand : IRequest<int>;

public class ProcessEmailQueueCommandHandler : IRequestHandler<ProcessEmailQueueCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ProcessEmailQueueCommandHandler> _logger;

    public ProcessEmailQueueCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService,
        ILogger<ProcessEmailQueueCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<int> Handle(ProcessEmailQueueCommand request, CancellationToken ct)
    {
        var pendingEmails = await _context.EmailQueues
            .Where(e => e.Status == EmailStatus.Pending && e.RetryCount < 3 && !e.IsDeleted)
            .Take(100)
            .ToListAsync(ct);

        int processedCount = 0;

        foreach (var email in pendingEmails)
        {
            try
            {
                var success = await _emailService.SendEmailAsync(
                    email.To,
                    email.Subject,
                    email.HtmlBody,
                    email.PlainTextBody,
                    ct
                );

                if (success)
                {
                    email.Status = EmailStatus.Sent;
                    email.SentAt = DateTime.UtcNow;
                    processedCount++;
                    _logger.LogInformation("Email sent to {To}", email.To);
                }
                else
                {
                    email.RetryCount++;
                    if (email.RetryCount >= 3)
                    {
                        email.Status = EmailStatus.Failed;
                        email.FailureReason = "Maximum retry count reached";
                    }
                    _logger.LogWarning("Failed to send email to {To}, retry count: {RetryCount}", email.To, email.RetryCount);
                }
            }
            catch (Exception ex)
            {
                email.RetryCount++;
                email.FailureReason = ex.Message;
                if (email.RetryCount >= 3)
                {
                    email.Status = EmailStatus.Failed;
                }
                _logger.LogError(ex, "Error sending email to {To}", email.To);
            }
        }

        await _context.SaveChangesAsync(ct);
        return processedCount;
    }
}
