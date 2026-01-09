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
        // Arrange
        var description = "Test Product";
        var supplier = CreateTestSupplier();
        var category = CreateTestCategory();
        var acquisitionCost = CreateTestPrice(100m, "USD");
        var acquisitionCostUSD = CreateTestPrice(100m, "USD");

        // Act
        var product = new Product(description, acquisitionCost, acquisitionCostUSD, supplier, category);

        // Assert
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
        // Arrange
        var description = "Test Product";
        var supplier = CreateTestSupplier();
        var category = CreateTestCategory();
        var acquisitionCost = CreateTestPrice();
        var acquisitionCostUSD = CreateTestPrice();

        // Act
        var product = new Product(description, acquisitionCost, acquisitionCostUSD, supplier, category);

        // Assert
        Assert.Single(product.Events);
        Assert.IsType<ProductCreatedEvent>(product.Events.First());
    }

    [Fact]
    public void SetWmsProductId_ShouldSetCorrectly()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        var wmsProductId = "wms-12345";

        // Act
        product.SetWmsProductId(wmsProductId);

        // Assert
        Assert.Equal(wmsProductId, product.WmsProductId);
    }

    [Fact]
    public void Sell_WhenProductIsCreated_ShouldSellSuccessfully()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.ClearEvents();

        // Act
        product.Sell();

        // Assert
        Assert.Equal(ProductStatus.Sold, product.Status);
        Assert.NotNull(product.SoldDate);
        Assert.Single(product.Events);
        Assert.IsType<ProductSoldEvent>(product.Events.First());
    }

    [Fact]
    public void Sell_WhenProductIsAlreadySold_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => product.Sell());
        Assert.Contains("already solded", exception.Message);
    }

    [Fact]
    public void Sell_WhenProductIsCanceled_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();
        product.Cancel();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => product.Sell());
        Assert.Contains("Cancelled and returned products cannot be sold", exception.Message);
    }

    [Fact]
    public void Sell_WhenProductIsReturned_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();
        product.Return();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => product.Sell());
        Assert.Contains("Cancelled and returned products cannot be sold", exception.Message);
    }

    [Fact]
    public void Return_WhenProductIsSold_ShouldReturnSuccessfully()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        // Act
        product.Return();

        // Assert
        Assert.Equal(ProductStatus.Returned, product.Status);
        Assert.NotNull(product.ReturnDate);
    }

    [Fact]
    public void Return_WhenProductIsNotSold_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => product.Return());
        Assert.Contains("Non sold products cannot be returned", exception.Message);
    }

    [Fact]
    public void Cancel_WhenProductIsSold_ShouldCancelSuccessfully()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        // Act
        product.Cancel();

        // Assert
        Assert.Equal(ProductStatus.Canceled, product.Status);
        Assert.NotNull(product.CancelDate);
    }

    [Fact]
    public void Cancel_WhenProductIsNotSold_ShouldThrowDomainException()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => product.Cancel());
        Assert.Contains("Non sold products cannot be cancelled", exception.Message);
    }
}
