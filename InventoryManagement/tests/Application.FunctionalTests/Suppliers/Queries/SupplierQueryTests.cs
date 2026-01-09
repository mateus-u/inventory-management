using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Suppliers.Queries;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Suppliers.Queries;

public class SupplierQueryTests : TestingBase
{
    [Fact]
    public async Task SupplierGetAllQuery_WithNoSuppliers_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new SupplierGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task SupplierGetAllQuery_WithSuppliers_ShouldReturnAllSuppliers()
    {
        // Arrange
        var supplier1 = new Supplier("Supplier 1", new Email("supplier1@test.com"),
            Currency.FromCode("USD"), Country.FromCode("US"));
        var supplier2 = new Supplier("Supplier 2", new Email("supplier2@test.com"),
            Currency.FromCode("EUR"), Country.FromCode("FR"));
        var supplier3 = new Supplier("Supplier 3", new Email("supplier3@test.com"),
            Currency.FromCode("GBP"), Country.FromCode("GB"));

        await ExecuteDbContextAsync(async context =>
        {
            context.Suppliers.AddRange(supplier1, supplier2, supplier3);
            await context.SaveChangesAsync();
        });

        var query = new SupplierGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Name == "Supplier 1");
        result.Should().Contain(s => s.Name == "Supplier 2");
        result.Should().Contain(s => s.Name == "Supplier 3");
    }

    [Fact]
    public async Task SupplierGetByIdQuery_WithExistingSupplier_ShouldReturnSupplier()
    {
        // Arrange
        var supplier = new Supplier("Test Supplier", new Email("test@supplier.com"),
            Currency.FromCode("USD"), Country.FromCode("US"));

        await ExecuteDbContextAsync(async context =>
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
        });

        var query = new SupplierGetByIdQuery { Id = supplier.Id };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(supplier.Id);
        result.Name.Should().Be("Test Supplier");
        result.Email.Should().Be("test@supplier.com");
        result.CurrencyCode.Should().Be("USD");
        result.CountryCode.Should().Be("US");
    }

    [Fact]
    public async Task SupplierGetByIdQuery_WithNonExistentSupplier_ShouldReturnNull()
    {
        // Arrange
        var query = new SupplierGetByIdQuery { Id = Guid.NewGuid() };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SupplierGetAllQuery_ShouldIncludeAllSupplierDetails()
    {
        // Arrange
        var supplier = new Supplier("Luxury Supplier", new Email("luxury@test.com"),
            Currency.FromCode("EUR"), Country.FromCode("FR"));

        await ExecuteDbContextAsync(async context =>
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
        });

        var query = new SupplierGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new SupplierGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(1);
        var supplierResult = result.First();
        supplierResult.Name.Should().Be("Luxury Supplier");
        supplierResult.Email.Should().Be("luxury@test.com");
        supplierResult.CurrencyCode.Should().Be("EUR");
        supplierResult.CountryCode.Should().Be("FR");
    }
}
