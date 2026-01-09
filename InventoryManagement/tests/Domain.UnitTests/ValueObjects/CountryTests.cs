using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class CountryTests
{
    [Fact]
    public void Country_WithValidCode_ShouldCreateSuccessfully()
    {
        // Arrange
        var validCode = "US";

        // Act
        var country = Country.FromCode(validCode);

        // Assert
        Assert.NotNull(country);
        Assert.Equal("US", country.Code);
    }

    [Theory]
    [InlineData("us")]
    [InlineData("Us")]
    [InlineData("pt")]
    public void Country_WithLowercaseOrMixedCase_ShouldConvertToUppercase(string code)
    {
        // Act
        var country = Country.FromCode(code);

        // Assert
        Assert.Equal(code.ToUpperInvariant(), country.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Country_WithEmptyOrWhiteSpaceCode_ShouldThrowArgumentException(string? invalidCode)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Country.FromCode(invalidCode!));
    }

    [Theory]
    [InlineData("U")]
    [InlineData("USA")]
    [InlineData("1234")]
    [InlineData("A")]
    public void Country_WithInvalidLength_ShouldThrowArgumentException(string invalidCode)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Country.FromCode(invalidCode));
        Assert.Contains("must be 2 characters", exception.Message);
    }

    [Theory]
    [InlineData("US")]
    [InlineData("GB")]
    [InlineData("BR")]
    [InlineData("FR")]
    [InlineData("PT")]
    public void Country_WithCommonIsoCodes_ShouldCreateSuccessfully(string code)
    {
        // Act
        var country = Country.FromCode(code);

        // Assert
        Assert.NotNull(country);
        Assert.Equal(code, country.Code);
    }

    [Fact]
    public void Country_RecordEquality_ShouldWorkCorrectly()
    {
        // Arrange
        var country1 = Country.FromCode("US");
        var country2 = Country.FromCode("US");
        var country3 = Country.FromCode("BR");

        // Assert
        Assert.Equal(country1, country2);
        Assert.NotEqual(country1, country3);
    }
}
