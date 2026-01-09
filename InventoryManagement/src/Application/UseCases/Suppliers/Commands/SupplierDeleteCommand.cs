using Application.Common.Interfaces;
using Application.Common.Mediator;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Suppliers.Commands;

public class SupplierDeleteCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class SupplierDeleteCommandValidator : AbstractValidator<SupplierDeleteCommand>
{
    public SupplierDeleteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");
    }
}

public class SupplierDeleteCommandHandler : IHandler<SupplierDeleteCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public SupplierDeleteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HandleAsync(SupplierDeleteCommand request, CancellationToken cancellationToken = default)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (supplier == null)
        {
            return false;
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
