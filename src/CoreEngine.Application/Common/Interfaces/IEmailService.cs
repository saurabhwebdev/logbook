namespace CoreEngine.Application.Common.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default);
    Task<bool> SendTemplatedEmailAsync(string templateName, string to, Dictionary<string, string> variables, CancellationToken ct = default);
    Task QueueEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null, CancellationToken ct = default);
}
