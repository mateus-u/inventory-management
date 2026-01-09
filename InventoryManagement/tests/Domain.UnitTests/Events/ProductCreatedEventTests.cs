using Domain.Entities;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.UnitTests.Events;

public class ProductCreatedEventTests
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
    public void ProductCreatedEvent_Constructor_ShouldSetProductCorrectly()
    {
        // Arrange
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());

        // Act
        var productCreatedEvent = new ProductCreatedEvent(product);

        // Assert
        Assert.NotNull(productCreatedEvent);
        Assert.Equal(product, productCreatedEvent.Product);
    }

    [Fact]
    public void ProductCreatedEvent_ShouldBeRaisedWhenProductIsCreated()
    {
        // Arrange & Act
        var product = new Product("Test Product", CreateTestPrice(), CreateTestPrice(), CreateTestSupplier(), CreateTestCategory());

        // Assert
        Assert.Single(product.Events);
        var domainEvent = product.Events.First();
        Assert.IsType<ProductCreatedEvent>(domainEvent);
        
        var productCreatedEvent = (ProductCreatedEvent)domainEvent;
        Assert.Equal(product, productCreatedEvent.Product);
    }
}
