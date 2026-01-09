using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Suppliers.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Suppliers.Queries;

public class SupplierGetByIdQuery : IRequest<SupplierResponse?>
{
    public Guid Id { get; set; }
}

public class SupplierGetByIdQueryHandler : IHandler<SupplierGetByIdQuery, SupplierResponse?>
{
    private readonly IApplicationDbContext _context;

    public SupplierGetByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierResponse?> HandleAsync(SupplierGetByIdQuery request, CancellationToken cancellationToken = default)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier == null)
        {
            return null;
        }

        return new SupplierResponse
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Email = supplier.Email.Address,
            CurrencyCode = supplier.Currency.Code,
            CountryCode = supplier.Country.Code,
            CreatedAt = supplier.CreatedAt,
        };
    }
}
