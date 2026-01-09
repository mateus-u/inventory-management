using Application.Common.Interfaces;
using Application.Common.Mediator;
using Application.UseCases.Suppliers.Models;
using Domain.Entities;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.UseCases.Suppliers.Commands;

public class SupplierCreateCommand : IRequest<SupplierResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}

public class SupplierCreateCommandValidator : AbstractValidator<SupplierCreateCommand>
{
    private readonly IApplicationDbContext _context;

    public SupplierCreateCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.")
            .Must(BeUniqueEmail).WithMessage("A supplier with this email already exists.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().WithMessage("Currency code is required.")
            .Length(3).WithMessage("Currency code must be 3 characters.");

        RuleFor(x => x.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2).WithMessage("Country code must be 2 characters.");
    }

    private bool BeUniqueEmail(string email)
    {
        return !_context.Suppliers.Any(s => s.Email.Address == email);
    }
}

public class SupplierCreateCommandHandler : IHandler<SupplierCreateCommand, SupplierResponse>
{
    private readonly IApplicationDbContext _context;

    public SupplierCreateCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SupplierResponse> HandleAsync(SupplierCreateCommand request, CancellationToken cancellationToken = default)
    {
        var email = new Email(request.Email);
        var currency = Currency.FromCode(request.CurrencyCode);
        var country = Country.FromCode(request.CountryCode);

        var supplier = new Supplier(request.Name, email, currency, country);

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

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
