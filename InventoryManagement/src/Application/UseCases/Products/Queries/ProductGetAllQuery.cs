using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Products.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Products.Queries;

public class ProductGetAllQuery : IRequest<List<ProductResponse>>
{
}

public class ProductGetAllQueryHandler : IHandler<ProductGetAllQuery, List<ProductResponse>>
{
    private readonly IApplicationDbContext _context;

    public ProductGetAllQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductResponse>> HandleAsync(ProductGetAllQuery request, CancellationToken cancellationToken = default)
    {
        var products = await _context.Products
            .Include(p => p.Supplier)
            .Include(p => p.Category)
            .ToListAsync(cancellationToken);

        return products.Select(product => new ProductResponse
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
            CreatedAt = product.CreatedAt,
        }).ToList();
    }
}
