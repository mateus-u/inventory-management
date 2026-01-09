using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Suppliers.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Suppliers.Queries;

public class SupplierGetAllQuery : IRequest<List<SupplierResponse>>
{
}

public class SupplierGetAllQueryHandler : IHandler<SupplierGetAllQuery, List<SupplierResponse>>
{
    private readonly IApplicationDbContext _context;

    public SupplierGetAllQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SupplierResponse>> HandleAsync(SupplierGetAllQuery request, CancellationToken cancellationToken = default)
    {
        return await _context.Suppliers
            .Select(supplier => new SupplierResponse
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email.Address,
                CurrencyCode = supplier.Currency.Code,
                CountryCode = supplier.Country.Code,
                CreatedAt = supplier.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
