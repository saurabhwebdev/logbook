using FluentValidation;

namespace CoreEngine.Application.Features.Emails.Commands.SendEmail;

public class SendEmailCommandValidator : AbstractValidator<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Recipient email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters.");

        RuleFor(x => x.HtmlBody)
            .NotEmpty().WithMessage("Email body is required.");
    }
}
