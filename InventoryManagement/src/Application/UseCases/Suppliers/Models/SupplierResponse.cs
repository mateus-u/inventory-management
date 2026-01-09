namespace Application.UseCases.Suppliers.Models;

public class SupplierResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
