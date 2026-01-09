using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Products.Models;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Products.Commands;

public class ProductCreateCommand : IRequest<ProductResponse>
{
    public string Description { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid CategoryId { get; set; }

    public decimal AcquisitionCost { get; set; }
    public decimal AcquisitionCostUSD { get; set; }
}

public class ProductCreateCommandValidator : AbstractValidator<ProductCreateCommand>
{
    public ProductCreateCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Supplier ID is required.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.AcquisitionCost)
            .GreaterThanOrEqualTo(0).WithMessage("Acquisition cost must be non-negative.");

        RuleFor(x => x.AcquisitionCostUSD)
            .GreaterThanOrEqualTo(0).WithMessage("Acquisition cost USD must be non-negative.");
    }
}

public class ProductCreateCommandHandler : IHandler<ProductCreateCommand, ProductResponse>
{
    private readonly IApplicationDbContext _context;

    public ProductCreateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> HandleAsync(ProductCreateCommand request, CancellationToken cancellationToken = default)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId, cancellationToken);

        if (supplier == null)
        {
            throw new InvalidOperationException($"Supplier with ID {request.SupplierId} not found.");
        }

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            throw new InvalidOperationException($"Category with ID {request.CategoryId} not found.");
        }

        var acquisitionCost = Price.Create(request.AcquisitionCost, supplier.Currency);
        var acquisitionCostUSD = Price.Create(request.AcquisitionCostUSD, Currency.FromCode("USD"));

        var product = new Product(request.Description, acquisitionCost, acquisitionCostUSD, supplier, category);

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new ProductResponse
        {
            Id = product.Id,
            Description = product.Description,
            SupplierId = product.Supplier.Id,
            SupplierName = product.Supplier.Name,
            CategoryId = product.Category.Id,
            CategoryName = product.Category.Name,
            Status = product.Status.ToString(),
            AcquireDate = product.AcquireDate,
            SoldDate = product.SoldDate,
            CancelDate = product.CancelDate,
            ReturnDate = product.ReturnDate,
            CreatedAt = product.CreatedAt,
        };
    }
}
