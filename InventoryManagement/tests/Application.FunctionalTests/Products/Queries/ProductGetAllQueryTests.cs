using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Products.Queries;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Products.Queries;

public class ProductGetAllQueryTests : TestingBase
{
    [Fact]
    public async Task ProductGetAllQuery_WithNoProducts_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new ProductGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task ProductGetAllQuery_WithProducts_ShouldReturnAllProducts()
    {
        // Arrange
        var supplier = new Supplier("Test Supplier", new Email("test@supplier.com"),
            Currency.FromCode("USD"), Country.FromCode("US"));
        var category = new Category("Electronics", "ELEC");
        var acquisitionCost = Price.Create(100m, Currency.FromCode("USD"));
        var acquisitionCostUSD = Price.Create(100m, Currency.FromCode("USD"));

        var product1 = new Product("Product 1", acquisitionCost, acquisitionCostUSD, supplier, category);
        var product2 = new Product("Product 2", acquisitionCost, acquisitionCostUSD, supplier, category);
        var product3 = new Product("Product 3", acquisitionCost, acquisitionCostUSD, supplier, category);

        await ExecuteDbContextAsync(async context =>
        {
            context.Products.AddRange(product1, product2, product3);
            await context.SaveChangesAsync();
        });

        var query = new ProductGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Description == "Product 1");
        result.Should().Contain(p => p.Description == "Product 2");
        result.Should().Contain(p => p.Description == "Product 3");
    }

    [Fact]
    public async Task ProductGetAllQuery_ShouldIncludeSupplierAndCategoryInfo()
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

        var query = new ProductGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(1);
        var productResult = result.First();
        productResult.SupplierName.Should().Be("Test Supplier");
        productResult.CategoryName.Should().Be("Electronics");
    }
}
