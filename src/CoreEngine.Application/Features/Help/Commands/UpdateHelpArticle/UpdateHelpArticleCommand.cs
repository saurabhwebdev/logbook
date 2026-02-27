using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Help.Commands.UpdateHelpArticle;

public record UpdateHelpArticleCommand(
    Guid Id,
    string Title,
    string Slug,
    string? ModuleKey,
    string Content,
    string? Category,
    int SortOrder,
    bool IsPublished,
    string? Tags
) : IRequest<Unit>;

public class UpdateHelpArticleCommandValidator : AbstractValidator<UpdateHelpArticleCommand>
{
    public UpdateHelpArticleCommandValidator()
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

public class UpdateHelpArticleCommandHandler : IRequestHandler<UpdateHelpArticleCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateHelpArticleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateHelpArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _context.HelpArticles
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (article is null)
            throw new NotFoundException(nameof(HelpArticle), request.Id);

        article.Title = request.Title;
        article.Slug = request.Slug;
        article.ModuleKey = request.ModuleKey;
        article.Content = request.Content;
        article.Category = request.Category;
        article.SortOrder = request.SortOrder;
        article.IsPublished = request.IsPublished;
        article.Tags = request.Tags;
        article.ModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
