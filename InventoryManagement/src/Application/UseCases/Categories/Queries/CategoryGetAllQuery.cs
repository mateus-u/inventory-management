using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Categories.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Categories.Queries;

public class CategoryGetAllQuery : IRequest<List<CategoryResponse>>
{
}

public class CategoryGetAllQueryHandler : IHandler<CategoryGetAllQuery, List<CategoryResponse>>
{
    private readonly IApplicationDbContext _context;

    public CategoryGetAllQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryResponse>> HandleAsync(CategoryGetAllQuery request, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Parent)
            .Select(category => new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Shortcode = category.Shortcode,
                ParentId = category.Parent != null ? category.Parent.Id : null,
                ParentName = category.Parent != null ? category.Parent.Name : null,
                CreatedAt = category.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
