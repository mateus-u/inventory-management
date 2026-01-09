using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Categories.Models;
using Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Categories.Commands;

public class CategoryCreateCommand : IRequest<CategoryResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Shortcode { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
}

public class CategoryCreateCommandValidator : AbstractValidator<CategoryCreateCommand>
{
    private readonly IApplicationDbContext _context;

    public CategoryCreateCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Must(BeUniqueName).WithMessage("A category with this name already exists.");

        RuleFor(x => x.Shortcode)
            .NotEmpty().WithMessage("Shortcode is required.")
            .Must(BeUniqueShortcode).WithMessage("A category with this shortcode already exists.");
    }

    private bool BeUniqueName(string name)
    {
        return !_context.Categories.Any(c => c.Name == name);
    }

    private bool BeUniqueShortcode(string shortcode)
    {
        return !_context.Categories.Any(c => c.Shortcode == shortcode);
    }
}

public class CategoryCreateCommandHandler : IHandler<CategoryCreateCommand, CategoryResponse>
{
    private readonly IApplicationDbContext _context;

    public CategoryCreateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryResponse> HandleAsync(CategoryCreateCommand request, CancellationToken cancellationToken = default)
    {
        Category? parent = null;

        if (request.ParentId.HasValue)
        {
            parent = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == request.ParentId.Value, cancellationToken);

            if (parent == null)
            {
                throw new ValidationException($"Parent category with ID {request.ParentId.Value} not found.");
            }
        }

        var category = new Category(request.Name, request.Shortcode, parent);

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync(cancellationToken);

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