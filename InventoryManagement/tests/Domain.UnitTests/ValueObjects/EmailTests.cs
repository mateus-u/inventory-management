using Domain.Common.Exceptions;
using Domain.ValueObjects;

namespace Domain.UnitTests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Email_WithValidAddress_ShouldCreateSuccessfully()
    {
        var validEmail = "test@example.com";

        var email = new Email(validEmail);

        Assert.NotNull(email);
        Assert.Equal(validEmail, email.Address);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Email_WithEmptyOrWhiteSpaceAddress_ShouldThrowArgumentException(string? invalidEmail)
    {
        Assert.Throws<DomainException>(() => new Email(invalidEmail!));
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("invalid.email")]
    public void Email_WithoutAtSymbol_ShouldThrowArgumentException(string invalidEmail)
    {
        Assert.Throws<DomainException>(() => new Email(invalidEmail));
    }

    [Theory]
    [InlineData("user@domain.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.org")]
    public void Email_WithVariousValidFormats_ShouldCreateSuccessfully(string validEmail)
    {
        var email = new Email(validEmail);

        Assert.NotNull(email);
        Assert.Equal(validEmail, email.Address);
    }
}
