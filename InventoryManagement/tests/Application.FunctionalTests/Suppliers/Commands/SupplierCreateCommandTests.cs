using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Suppliers.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Suppliers.Commands;

public class SupplierCreateCommandTests : TestingBase
{
    [Fact]
    public async Task SupplierCreateCommand_WithValidData_ShouldCreateSupplier()
    {
        // Arrange
        var command = new SupplierCreateCommand
        {
            Name = "Luxury Supplier",
            Email = "luxury@supplier.com",
            CurrencyCode = "USD",
            CountryCode = "US"
        };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Luxury Supplier");
        result.Email.Should().Be("luxury@supplier.com");
        result.CurrencyCode.Should().Be("USD");
        result.CountryCode.Should().Be("US");

        var savedSupplier = await ExecuteDbContextAsync(async context =>
            await context.Suppliers.FirstOrDefaultAsync(s => s.Id == result.Id));

        savedSupplier.Should().NotBeNull();
        savedSupplier!.Name.Should().Be("Luxury Supplier");
    }

    [Fact]
    public async Task SupplierCreateCommand_WithInvalidEmail_ShouldThrowException()
    {
        // Arrange
        var command = new SupplierCreateCommand
        {
            Name = "Test Supplier",
            Email = "invalid-email", // Invalid email format
            CurrencyCode = "USD",
            CountryCode = "US"
        };

        // Act & Assert
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task SupplierCreateCommand_WithInvalidCurrencyCode_ShouldThrowException()
    {
        // Arrange
        var command = new SupplierCreateCommand
        {
            Name = "Test Supplier",
            Email = "test@supplier.com",
            CurrencyCode = "US", // Should be 3 characters
            CountryCode = "US"
        };

        // Act & Assert
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task SupplierCreateCommand_WithInvalidCountryCode_ShouldThrowException()
    {
        // Arrange
        var command = new SupplierCreateCommand
        {
            Name = "Test Supplier",
            Email = "test@supplier.com",
            CurrencyCode = "USD",
            CountryCode = "USA" // Should be 2 characters
        };

        // Act & Assert
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<ArgumentException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task SupplierCreateCommand_WithEmptyName_ShouldFailValidation()
    {
        // Arrange
        var command = new SupplierCreateCommand
        {
            Name = "",
            Email = "test@supplier.com",
            CurrencyCode = "USD",
            CountryCode = "US"
        };

        using var scope = ServiceProvider.CreateScope();
        var validator = new SupplierCreateCommandValidator(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public async Task SupplierCreateCommand_WithInvalidEmailFormat_ShouldFailValidation()
    {
        // Arrange
        var command = new SupplierCreateCommand
        {
            Name = "Test Supplier",
            Email = "not-an-email",
            CurrencyCode = "USD",
            CountryCode = "US"
        };

        using var scope = ServiceProvider.CreateScope();
        var validator = new SupplierCreateCommandValidator(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
    }
}
