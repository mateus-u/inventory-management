using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Categories.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Categories.Queries;

public class CategoryGetByIdQuery : IRequest<CategoryResponse?>
{
    public Guid Id { get; set; }
}

public class CategoryGetByIdQueryHandler : IHandler<CategoryGetByIdQuery, CategoryResponse?>
{
    private readonly IApplicationDbContext _context;

    public CategoryGetByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponse?> HandleAsync(CategoryGetByIdQuery request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .Include(c => c.Parent)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (category == null)
        {
            return null;
        }

        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Shortcode = category.Shortcode,
            ParentId = category.Parent?.Id,
            ParentName = category.Parent?.Name,
            CreatedAt = category.CreatedAt,
        };
    }
}
