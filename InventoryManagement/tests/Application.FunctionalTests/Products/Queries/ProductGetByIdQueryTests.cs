using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Products.Queries;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Products.Queries;

public class ProductGetByIdQueryTests : TestingBase
{
    [Fact]
    public async Task ProductGetByIdQuery_WithExistingProduct_ShouldReturnProduct()
    {
        // Arrange
        var supplier = new Supplier("Test Supplier", new Email("test@supplier.com"),
            Currency.FromCode("USD"), Country.FromCode("US"));
        var category = new Category("Electronics", "ELEC");
        var acquisitionCost = Price.Create(100m, Currency.FromCode("USD"));
        var acquisitionCostUSD = Price.Create(100m, Currency.FromCode("USD"));
        var product = new Product("Test Product", acquisitionCost, acquisitionCostUSD, supplier, category);

        await ExecuteDbContextAsync(async context =>
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
        });

        var query = new ProductGetByIdQuery { Id = product.Id };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Description.Should().Be("Test Product");
        result.SupplierName.Should().Be("Test Supplier");
        result.CategoryName.Should().Be("Electronics");
    }

    [Fact]
    public async Task ProductGetByIdQuery_WithNonExistentProduct_ShouldReturnNull()
    {
        // Arrange
        var query = new ProductGetByIdQuery { Id = Guid.NewGuid() };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ProductGetByIdQuery_ShouldIncludeAllProductDetails()
    {
        // Arrange
        var supplier = new Supplier("Luxury Supplier", new Email("luxury@test.com"),
            Currency.FromCode("EUR"), Country.FromCode("FR"));
        var category = new Category("Jewelry", "JEWEL");
        var acquisitionCost = Price.Create(500m, Currency.FromCode("EUR"));
        var acquisitionCostUSD = Price.Create(550m, Currency.FromCode("USD"));
        var product = new Product("Diamond Ring", acquisitionCost, acquisitionCostUSD, supplier, category);
        product.Sell();

        await ExecuteDbContextAsync(async context =>
        {
            context.Products.Add(product);
            await context.SaveChangesAsync();
        });

        var query = new ProductGetByIdQuery { Id = product.Id };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Description.Should().Be("Diamond Ring");
        result.Status.Should().Be("Sold");
        result.SoldDate.Should().NotBeNull();
        result.AcquireDate.Should().NotBeNull();
    }
}
