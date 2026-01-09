using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.UnitTests.Entities;

public class SupplierTests
{
    [Fact]
    public void Supplier_Constructor_ShouldCreateSupplierWithCorrectProperties()
    {
        // Arrange
        var name = "Test Supplier";
        var email = new Email("supplier@test.com");
        var currency = Currency.FromCode("USD");
        var country = Country.FromCode("US");

        // Act
        var supplier = new Supplier(name, email, currency, country);

        // Assert
        Assert.NotEqual(Guid.Empty, supplier.Id);
        Assert.Equal(name, supplier.Name);
        Assert.Equal(email, supplier.Email);
        Assert.Equal(currency, supplier.Currency);
        Assert.Equal(country, supplier.Country);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Supplier_WithEmptyOrWhiteSpaceName_ShouldThrowDomainException(string? invalidName)
    {
        // Arrange
        var email = new Email("supplier@test.com");
        var currency = Currency.FromCode("USD");
        var country = Country.FromCode("US");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            new Supplier(invalidName!, email, currency, country));
        Assert.Contains("Name cannot be empty", exception.Message);
    }

    [Fact]
    public void Supplier_WithNullEmail_ShouldThrowDomainException()
    {
        // Arrange
        var name = "Test Supplier";
        var currency = Currency.FromCode("USD");
        var country = Country.FromCode("US");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            new Supplier(name, null!, currency, country));
        Assert.Contains("Email cannot be null", exception.Message);
    }

    [Fact]
    public void Supplier_WithNullCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var name = "Test Supplier";
        var email = new Email("supplier@test.com");
        var country = Country.FromCode("US");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            new Supplier(name, email, null!, country));
        Assert.Contains("Currency cannot be null", exception.Message);
    }

    [Fact]
    public void Supplier_WithNullCountry_ShouldThrowDomainException()
    {
        // Arrange
        var name = "Test Supplier";
        var email = new Email("supplier@test.com");
        var currency = Currency.FromCode("USD");

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => 
            new Supplier(name, email, currency, null!));
        Assert.Contains("Country cannot be null", exception.Message);
    }

    [Theory]
    [InlineData("Luxury Goods Inc", "luxury@example.com", "EUR", "FR")]
    [InlineData("Asian Imports", "contact@asianimports.com", "JPY", "JP")]
    [InlineData("Brazilian Exports", "sales@brexports.com", "BRL", "BR")]
    public void Supplier_WithVariousValidData_ShouldCreateSuccessfully(
        string name, string emailAddress, string currencyCode, string countryCode)
    {
        // Arrange
        var email = new Email(emailAddress);
        var currency = Currency.FromCode(currencyCode);
        var country = Country.FromCode(countryCode);

        // Act
        var supplier = new Supplier(name, email, currency, country);

        // Assert
        Assert.NotNull(supplier);
        Assert.Equal(name, supplier.Name);
        Assert.Equal(emailAddress, supplier.Email.Address);
        Assert.Equal(currencyCode, supplier.Currency.Code);
        Assert.Equal(countryCode, supplier.Country.Code);
    }
}
