namespace Application.UseCases.Products.Models;

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal AcquisitionCost { get; set; }
    public decimal AcquisitionCostUSD { get; set; }
    public DateTime? AcquireDate { get; set; }
    public DateTime? SoldDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
