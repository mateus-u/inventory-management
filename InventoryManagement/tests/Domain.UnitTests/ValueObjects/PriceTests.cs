using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class PriceTests
{
    [Fact]
    public void Create_WithValidAmountAndCurrency_ShouldCreatePrice()
    {
        // Arrange
        var amount = 100.50m;
        var currency = Currency.FromCode("USD");

        // Act
        var price = Price.Create(amount, currency);

        // Assert
        Assert.Equal(amount, price.Amount);
        Assert.Equal(currency, price.Currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var amount = -10m;
        var currency = Currency.FromCode("EUR");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Price.Create(amount, currency));
        Assert.Contains("Amount cannot be negative", exception.Message);
    }

    [Fact]
    public void Create_WithNullCurrency_ShouldThrowArgumentNullException()
    {
        // Arrange
        var amount = 50m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() => Price.Create(amount, null!));
        Assert.Contains("Currency cannot be null", exception.Message);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldCreatePrice()
    {
        // Arrange
        var amount = 0m;
        var currency = Currency.FromCode("GBP");

        // Act
        var price = Price.Create(amount, currency);

        // Assert
        Assert.Equal(amount, price.Amount);
        Assert.Equal(currency, price.Currency);
    }

    [Fact]
    public void Price_ShouldBeEqualWhenAmountAndCurrencyAreEqual()
    {
        // Arrange
        var currency = Currency.FromCode("USD");
        var price1 = Price.Create(100m, currency);
        var price2 = Price.Create(100m, currency);

        // Act & Assert
        Assert.Equal(price1, price2);
    }

    [Fact]
    public void Price_ShouldNotBeEqualWhenAmountDiffers()
    {
        // Arrange
        var currency = Currency.FromCode("USD");
        var price1 = Price.Create(100m, currency);
        var price2 = Price.Create(200m, currency);

        // Act & Assert
        Assert.NotEqual(price1, price2);
    }

    [Fact]
    public void Price_ShouldNotBeEqualWhenCurrencyDiffers()
    {
        // Arrange
        var price1 = Price.Create(100m, Currency.FromCode("USD"));
        var price2 = Price.Create(100m, Currency.FromCode("EUR"));

        // Act & Assert
        Assert.NotEqual(price1, price2);
    }
}
