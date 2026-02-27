using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Enums;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace CoreEngine.Infrastructure.Services;

public class SmtpEmailService : IEmailService
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IApplicationDbContext context, ILogger<SmtpEmailService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default)
    {
        try
        {
            var smtpConfig = await GetSmtpConfigurationAsync(ct);
            if (smtpConfig == null)
            {
                _logger.LogError("SMTP configuration not found");
                return false;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpConfig.FromName, smtpConfig.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = plainTextBody ?? htmlBody
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpConfig.Host, smtpConfig.Port, smtpConfig.EnableSsl, ct);

            if (!string.IsNullOrEmpty(smtpConfig.Username) && !string.IsNullOrEmpty(smtpConfig.Password))
            {
                await client.AuthenticateAsync(smtpConfig.Username, smtpConfig.Password, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            _logger.LogInformation("Email sent successfully to {To}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            return false;
        }
    }

    public async Task<bool> SendTemplatedEmailAsync(string templateName, string to, Dictionary<string, string> variables, CancellationToken ct = default)
    {
        try
        {
            var template = await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Name == templateName && t.IsActive && !t.IsDeleted, ct);

            if (template == null)
            {
                _logger.LogError("Email template {TemplateName} not found", templateName);
                return false;
            }

            var subject = ReplaceVariables(template.Subject, variables);
            var htmlBody = ReplaceVariables(template.HtmlBody, variables);
            var plainTextBody = template.PlainTextBody != null ? ReplaceVariables(template.PlainTextBody, variables) : null;

            return await SendEmailAsync(to, subject, htmlBody, plainTextBody, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send templated email {TemplateName} to {To}", templateName, to);
            return false;
        }
    }

    public async Task QueueEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default)
    {
        var emailQueue = new EmailQueue
        {
            Id = Guid.NewGuid(),
            To = to,
            Subject = subject,
            HtmlBody = htmlBody,
            PlainTextBody = plainTextBody,
            Status = EmailStatus.Pending,
            RetryCount = 0
        };

        _context.EmailQueues.Add(emailQueue);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Email queued for {To}", to);
    }

    private async Task<SmtpConfiguration?> GetSmtpConfigurationAsync(CancellationToken ct)
    {
        var configs = await _context.SystemConfigurations
            .Where(c => c.Key.StartsWith("Email.Smtp") && !c.IsDeleted)
            .ToListAsync(ct);

        if (!configs.Any())
            return null;

        return new SmtpConfiguration
        {
            Host = configs.FirstOrDefault(c => c.Key == "Email.SmtpHost")?.Value ?? "",
            Port = int.Parse(configs.FirstOrDefault(c => c.Key == "Email.SmtpPort")?.Value ?? "587"),
            Username = configs.FirstOrDefault(c => c.Key == "Email.SmtpUsername")?.Value,
            Password = configs.FirstOrDefault(c => c.Key == "Email.SmtpPassword")?.Value,
            EnableSsl = bool.Parse(configs.FirstOrDefault(c => c.Key == "Email.SmtpEnableSsl")?.Value ?? "true"),
            FromAddress = configs.FirstOrDefault(c => c.Key == "Email.FromAddress")?.Value ?? "",
            FromName = configs.FirstOrDefault(c => c.Key == "Email.FromName")?.Value ?? ""
        };
    }

    private string ReplaceVariables(string content, Dictionary<string, string> variables)
    {
        foreach (var variable in variables)
        {
            content = content.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }
        return content;
    }

    private class SmtpConfiguration
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool EnableSsl { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
