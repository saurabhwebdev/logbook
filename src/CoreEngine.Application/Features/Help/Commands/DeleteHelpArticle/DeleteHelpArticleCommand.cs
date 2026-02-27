using CoreEngine.Application.Common.Exceptions;
using CoreEngine.Application.Common.Interfaces;
using CoreEngine.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CoreEngine.Application.Features.Help.Commands.DeleteHelpArticle;

public record DeleteHelpArticleCommand(Guid Id) : IRequest<Unit>;

public class DeleteHelpArticleCommandHandler : IRequestHandler<DeleteHelpArticleCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteHelpArticleCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteHelpArticleCommand request, CancellationToken cancellationToken)
    {
        var article = await _context.HelpArticles
            .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

        if (article is null)
            throw new NotFoundException(nameof(HelpArticle), request.Id);

        _context.HelpArticles.Remove(article);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
