using Domain.Common.Exceptions;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class PriceTests
{
    [Fact]
    public void Create_WithValidAmountAndCurrency_ShouldCreatePrice()
    {
        var amount = 100.50m;
        var currency = Currency.FromCode("USD");

        var price = Price.Create(amount, currency);

        Assert.Equal(amount, price.Amount);
        Assert.Equal(currency, price.Currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowArgumentException()
    {
        var amount = -10m;
        var currency = Currency.FromCode("EUR");

        var exception = Assert.Throws<DomainException>(() => Price.Create(amount, currency));
        Assert.Contains("Amount cannot be negative", exception.Message);
    }

    [Fact]
    public void Create_WithNullCurrency_ShouldThrowArgumentNullException()
    {
        var amount = 50m;

        var exception = Assert.Throws<DomainException>(() => Price.Create(amount, null!));
        Assert.Contains("Currency cannot be null", exception.Message);
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldCreatePrice()
    {
        var amount = 0m;
        var currency = Currency.FromCode("GBP");

        var price = Price.Create(amount, currency);

        Assert.Equal(amount, price.Amount);
        Assert.Equal(currency, price.Currency);
    }

    [Fact]
    public void Price_ShouldBeEqualWhenAmountAndCurrencyAreEqual()
    {
        var currency = Currency.FromCode("USD");
        var price1 = Price.Create(100m, currency);
        var price2 = Price.Create(100m, currency);

        Assert.Equal(price1, price2);
    }

    [Fact]
    public void Price_ShouldNotBeEqualWhenAmountDiffers()
    {
        var currency = Currency.FromCode("USD");
        var price1 = Price.Create(100m, currency);
        var price2 = Price.Create(200m, currency);

        Assert.NotEqual(price1, price2);
    }

    [Fact]
    public void Price_ShouldNotBeEqualWhenCurrencyDiffers()
    {
        var price1 = Price.Create(100m, Currency.FromCode("USD"));
        var price2 = Price.Create(100m, Currency.FromCode("EUR"));

        Assert.NotEqual(price1, price2);
    }
}
