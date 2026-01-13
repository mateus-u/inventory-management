using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Categories.Commands;

public class CategoryDeleteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class CategoryDeleteCommandValidator : AbstractValidator<CategoryDeleteCommand>
{
    public CategoryDeleteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}

public class CategoryDeleteCommandHandler : IHandler<CategoryDeleteCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public CategoryDeleteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(CategoryDeleteCommand request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
