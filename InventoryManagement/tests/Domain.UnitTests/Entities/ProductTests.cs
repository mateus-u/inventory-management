using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.UnitTests.Entities;

public class ProductTests
{
    private Supplier CreateTestSupplier()
    {
        return new Supplier(
            "Test Supplier",
            new Email("supplier@test.com"),
            Currency.FromCode("USD"),
            Country.FromCode("US")
        );
    }

    private Category CreateTestCategory()
    {
        return new Category("Electronics", "ELEC");
    }

    private Price CreateTestPrice(decimal amount = 100m, string currencyCode = "USD")
    {
        return Price.Create(amount, Currency.FromCode(currencyCode));
    }

    [Fact]
    public void Product_Constructor_ShouldCreateProductWithCorrectProperties()
    {
        var description = "Test Product";
        var supplier = CreateTestSupplier();
        var category = CreateTestCategory();
        var acquisitionCost = CreateTestPrice(100m, "USD");
        var acquisitionCostUSD = CreateTestPrice(100m, "USD");

        var product = new Product(description, acquisitionCost, acquisitionCostUSD, supplier, category);

        Assert.NotEqual(Guid.Empty, product.Id);
        Assert.Equal(description, product.Description);
        Assert.Equal(supplier, product.Supplier);
        Assert.Equal(category, product.Category);
        Assert.Equal(ProductStatus.Created, product.Status);
        Assert.NotNull(product.AcquireDate);
        Assert.Null(product.SoldDate);
        Assert.Null(product.CancelDate);
        Assert.Null(product.ReturnDate);
        Assert.Null(product.WmsProductId);
    }

    [Fact]
    public void Product_Constructor_ShouldAddProductCreatedEvent()
    {
        var description = "Test Product";
        var supplier = CreateTestSupplier();
        var category = CreateTestCategory();
        var acquisitionCost = CreateTestPrice();
        var acquisitionCostUSD = CreateTestPrice();

        var product = new Product(description, acquisitionCost, acquisitionCostUSD, supplier, category);

        Assert.Single(product.Events);
        Assert.IsType<ProductCreatedEvent>(product.Events.First());
    }

    [Fact]
    public void SetWmsProductId_ShouldSetCorrectly()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        var wmsProductId = "wms-12345";

        product.SetWmsProductId(wmsProductId);

        Assert.Equal(wmsProductId, product.WmsProductId);
    }

    [Fact]
    public void Sell_WhenProductIsCreated_ShouldSellSuccessfully()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.ClearEvents();

        product.Sell();

        Assert.Equal(ProductStatus.Sold, product.Status);
        Assert.NotNull(product.SoldDate);
        Assert.Single(product.Events);
        Assert.IsType<ProductSoldEvent>(product.Events.First());
    }

    [Fact]
    public void Sell_WhenProductIsAlreadySold_ShouldThrowDomainException()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        var exception = Assert.Throws<DomainException>(() => product.Sell());
        Assert.Contains("This product has already been sold", exception.Message);
    }

    [Fact]
    public void Sell_WhenProductIsCanceled_ShouldThrowDomainException()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();
        product.Cancel();

        var exception = Assert.Throws<DomainException>(() => product.Sell());
        Assert.Contains("Cancelled and returned products cannot be sold", exception.Message);
    }

    [Fact]
    public void Sell_WhenProductIsReturned_ShouldThrowDomainException()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();
        product.Return();

        var exception = Assert.Throws<DomainException>(() => product.Sell());
        Assert.Contains("Cancelled and returned products cannot be sold", exception.Message);
    }

    [Fact]
    public void Return_WhenProductIsSold_ShouldReturnSuccessfully()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        product.Return();

        Assert.Equal(ProductStatus.Returned, product.Status);
        Assert.NotNull(product.ReturnDate);
    }

    [Fact]
    public void Return_WhenProductIsNotSold_ShouldThrowDomainException()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());

        var exception = Assert.Throws<DomainException>(() => product.Return());
        Assert.Contains("Non sold products cannot be returned", exception.Message);
    }

    [Fact]
    public void Cancel_WhenProductIsSold_ShouldCancelSuccessfully()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        product.Cancel();

        Assert.Equal(ProductStatus.Canceled, product.Status);
        Assert.NotNull(product.CancelDate);
    }

    [Fact]
    public void Cancel_WhenProductIsNotSold_ShouldThrowDomainException()
    {
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());

        var exception = Assert.Throws<DomainException>(() => product.Cancel());
        Assert.Contains("Non sold products cannot be cancelled", exception.Message);
    }
}
