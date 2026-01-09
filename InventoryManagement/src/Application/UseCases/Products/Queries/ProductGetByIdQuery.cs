using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Products.Queries;

public class ProductGetByIdQuery : IRequest<ProductResponse?>
{
    public Guid Id { get; set; }
}

public class ProductGetByIdQueryHandler : IHandler<ProductGetByIdQuery, ProductResponse?>
{
    private readonly IApplicationDbContext _context;

    public ProductGetByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductResponse?> HandleAsync(ProductGetByIdQuery request, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            return null;
        }

        return new ProductResponse
        {
            Id = product.Id,
            Description = product.Description,
            SupplierId = product.Supplier.Id,
            SupplierName = product.Supplier.Name,
            CategoryId = product.Category.Id,
            CategoryName = product.Category.Name,
            Status = product.Status.ToString(),
            AcquisitionCost = product.AcquisitionCost?.Amount ?? 0,
            AcquisitionCostUSD = product.AcquisitionCostUSD?.Amount ?? 0,
            AcquireDate = product.AcquireDate,
            SoldDate = product.SoldDate,
            CancelDate = product.CancelDate,
            ReturnDate = product.ReturnDate,
            CreatedAt = product.CreatedAt
        };
    }
}
