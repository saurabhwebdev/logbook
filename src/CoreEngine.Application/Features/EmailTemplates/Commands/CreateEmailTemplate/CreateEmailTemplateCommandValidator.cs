using FluentValidation;

namespace CoreEngine.Application.Features.EmailTemplates.Commands.CreateEmailTemplate;

public class CreateEmailTemplateCommandValidator : AbstractValidator<CreateEmailTemplateCommand>
{
    public CreateEmailTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(500).WithMessage("Subject must not exceed 500 characters.");

        RuleFor(x => x.HtmlBody)
            .NotEmpty().WithMessage("HTML body is required.");
    }
}
