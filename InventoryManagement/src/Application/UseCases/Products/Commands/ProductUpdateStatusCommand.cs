using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Products.Models;
using Domain.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Products.Commands;

public class ProductUpdateStatusCommand : IRequest<ProductResponse>
{
    private Guid Id { get; set; }
    public ProductStatus Status { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public Guid GetId()
    {
        return Id;
    }
}

public class ProductUpdateCommandValidator : AbstractValidator<ProductUpdateStatusCommand>
{
    public ProductUpdateCommandValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}

public class ProductUpdateCommandHandler : IHandler<ProductUpdateStatusCommand, ProductResponse>
{
    private readonly IApplicationDbContext _context;

    public ProductUpdateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse> HandleAsync(ProductUpdateStatusCommand request, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.GetId(), cancellationToken);

        if (product == null)
        {
            throw new InvalidOperationException($"Product with ID {request.GetId()} not found.");
        }

        switch(request.Status)
        {
            case ProductStatus.Sold:
                product.Sell();
                break;
            case ProductStatus.Returned:
                product.Return();
                break;
            case ProductStatus.Canceled:
                product.Cancel();
                break;
            default:
                throw new ValidationException($"Invalid status transition to {request.Status}.");
        }

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
