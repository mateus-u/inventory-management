using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class CurrencyTests
{
    [Fact]
    public void Currency_WithValidCode_ShouldCreateSuccessfully()
    {
        // Arrange
        var validCode = "USD";

        // Act
        var currency = Currency.FromCode(validCode);

        // Assert
        Assert.NotNull(currency);
        Assert.Equal("USD", currency.Code);
    }

    [Theory]
    [InlineData("usd")]
    [InlineData("UsD")]
    [InlineData("eur")]
    public void Currency_WithLowercaseOrMixedCase_ShouldConvertToUppercase(string code)
    {
        // Act
        var currency = Currency.FromCode(code);

        // Assert
        Assert.Equal(code.ToUpperInvariant(), currency.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Currency_WithEmptyOrWhiteSpaceCode_ShouldThrowArgumentException(string? invalidCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Currency.FromCode(invalidCode!));
    }

    [Theory]
    [InlineData("US")]
    [InlineData("U")]
    [InlineData("USDD")]
    [InlineData("12345")]
    public void Currency_WithInvalidLength_ShouldThrowArgumentException(string invalidCode)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Currency.FromCode(invalidCode));
        Assert.Contains("must be 3 characters", exception.Message);
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    [InlineData("BRL")]
    public void Currency_WithCommonIsoCodes_ShouldCreateSuccessfully(string code)
    {
        // Act
        var currency = Currency.FromCode(code);

        // Assert
        Assert.NotNull(currency);
        Assert.Equal(code, currency.Code);
    }

    [Fact]
    public void Currency_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var currency1 = Currency.FromCode("USD");
        var currency2 = Currency.FromCode("USD");
        var currency3 = Currency.FromCode("EUR");

        // Assert
        Assert.Equal(currency1, currency2);
        Assert.NotEqual(currency1, currency3);
    }
}
