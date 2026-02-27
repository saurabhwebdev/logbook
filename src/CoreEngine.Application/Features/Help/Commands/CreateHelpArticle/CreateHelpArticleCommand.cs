using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using CoreEngine.Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace CoreEngine.Application.Features.Help.Commands.CreateHelpArticle;

public record CreateHelpArticleCommand(
    string Title,
    string Slug,
    string? ModuleKey,
    string Content,
    string? Category,
    int SortOrder,
    bool IsPublished,
    string? Tags
) : IRequest<Guid>;

public class CreateHelpArticleCommandValidator : AbstractValidator<CreateHelpArticleCommand>
{
    public CreateHelpArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(200).WithMessage("Slug must not exceed 200 characters.")
            .Matches(@"^[a-z0-9]+(-[a-z0-9]+)*$").WithMessage("Slug must be lowercase alphanumeric with hyphens only (e.g. 'my-article').");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters.");

        RuleFor(x => x.Tags)
            .MaximumLength(500).WithMessage("Tags must not exceed 500 characters.");
    }
}

public class CreateHelpArticleCommandHandler : IRequestHandler<CreateHelpArticleCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ITenantContext _tenantContext;

    public CreateHelpArticleCommandHandler(IApplicationDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }

    public async Task<Guid> Handle(CreateHelpArticleCommand request, CancellationToken cancellationToken)
    {
        var article = new HelpArticle
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Slug = request.Slug,
            ModuleKey = request.ModuleKey,
            Content = request.Content,
            Category = request.Category,
            SortOrder = request.SortOrder,
            IsPublished = request.IsPublished,
            Tags = request.Tags,
            TenantId = _tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.HelpArticles.Add(article);
        await _context.SaveChangesAsync(cancellationToken);

        return article.Id;
    }
}
