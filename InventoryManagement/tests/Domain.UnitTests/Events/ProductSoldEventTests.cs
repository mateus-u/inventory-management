using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.UnitTests.Events;

public class ProductSoldEventTests
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
    public void ProductSoldEvent_Constructor_ShouldSetProductCorrectly()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.Sell();

        // Act
        var productSoldEvent = new ProductSoldEvent(product);

        // Assert
        Assert.NotNull(productSoldEvent);
        Assert.Equal(product, productSoldEvent.Product);
    }

    [Fact]
    public void ProductSoldEvent_ShouldBeRaisedWhenProductIsSold()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());
        product.ClearEvents(); // Clear the ProductCreatedEvent

        // Act
        product.Sell();

        // Assert
        Assert.Single(product.Events);
        var domainEvent = product.Events.First();
        Assert.IsType<ProductSoldEvent>(domainEvent);
        
        var productSoldEvent = (ProductSoldEvent)domainEvent;
        Assert.Equal(product, productSoldEvent.Product);
    }
}
