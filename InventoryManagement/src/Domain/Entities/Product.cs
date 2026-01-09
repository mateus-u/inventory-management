using Domain.Common.Entities;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public Guid Id { get; protected set; }

    public Supplier Supplier { get; protected set; }
    public Category Category { get; protected set; }

    public string Description { get; protected set; }
    public string? WmsProductId { get; private set; }

    public Price AcquisitionCost { get; protected set; }
    public Price AcquisitionCostUSD { get; protected set; }
    
    public ProductStatus Status { get; protected set; }

    public DateTime? AcquireDate { get; protected set; }
    public DateTime? SoldDate { get; protected set; }
    public DateTime? CancelDate { get; protected set; }
    public DateTime? ReturnDate { get; protected set; }

    protected Product() 
    {
        Description = string.Empty;
        Supplier = null!;
        Category = null!;
        AcquisitionCost = null!;
        AcquisitionCostUSD = null!;
    }

    public Product(string description, Price acquisitionCost, Price acquisitionCostUSD, Supplier supplier, Category category) : base()
    {
        Id = Guid.NewGuid();

        Description = description;

        AcquisitionCost = acquisitionCost;
        AcquisitionCostUSD = acquisitionCostUSD;
        
        Supplier = supplier;
        Category = category;

        Status = ProductStatus.Created;
        AcquireDate = DateTime.UtcNow;

        AddEvent(new ProductCreatedEvent(this));
    }

    public void SetWmsProductId(string wmsProductId)
    {
        WmsProductId = wmsProductId;
    }

    public void Sell()
    {
        if (Status == ProductStatus.Canceled || Status == ProductStatus.Returned)
        {
            throw new DomainException("Cancelled and returned products cannot be sold");
        }

        if(Status == ProductStatus.Sold)
        {
            throw new DomainException("Product already solded");
        }

        Status = ProductStatus.Sold;
        SoldDate = DateTime.UtcNow;


        AddEvent(new ProductSoldEvent(this));
    }

    public void Return()
    {
        if (Status != ProductStatus.Sold)
        {
            throw new DomainException("Non sold products cannot be returned");
        }

        Status = ProductStatus.Returned;
        ReturnDate = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != ProductStatus.Sold)
        {
            throw new DomainException("Non sold products cannot be cancelled");
        }

        Status = ProductStatus.Canceled;
        CancelDate = DateTime.UtcNow;
    }
}
