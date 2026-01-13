using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Products.Commands;
using Domain.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Products.Commands;

public class ProductUpdateStatusCommandTests : TestingBase
{
    private async Task<Product> CreateTestProduct()
    {
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

        return product;
    }

    [Fact]
    public async Task ProductUpdateStatusCommand_SellProduct_ShouldUpdateStatus()
    {
        var product = await CreateTestProduct();

        var command = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Sold
        };
        command.SetId(product.Id);

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductUpdateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Status.Should().Be(ProductStatus.Sold.ToString());
        result.SoldDate.Should().NotBeNull();

        var updatedProduct = await ExecuteDbContextAsync(async context =>
            await context.Products.FirstOrDefaultAsync(p => p.Id == product.Id));

        updatedProduct.Should().NotBeNull();
        updatedProduct!.Status.Should().Be(ProductStatus.Sold);
        updatedProduct.SoldDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ProductUpdateStatusCommand_ReturnProduct_ShouldUpdateStatus()
    {
        var product = await CreateTestProduct();
        
        var sellCommand = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Sold
        };
        sellCommand.SetId(product.Id);
        
        using (var sellScope = ServiceProvider.CreateScope())
        {
            var sellHandler = new ProductUpdateCommandHandler(
                sellScope.ServiceProvider.GetRequiredService<IApplicationDbContext>());
            await sellHandler.HandleAsync(sellCommand);
        }

        var command = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Returned
        };
        command.SetId(product.Id);

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductUpdateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Status.Should().Be(ProductStatus.Returned.ToString());
        result.ReturnDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ProductUpdateStatusCommand_CancelProduct_ShouldUpdateStatus()
    {
        var product = await CreateTestProduct();
        
        var sellCommand = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Sold
        };
        sellCommand.SetId(product.Id);
        
        using (var sellScope = ServiceProvider.CreateScope())
        {
            var sellHandler = new ProductUpdateCommandHandler(
                sellScope.ServiceProvider.GetRequiredService<IApplicationDbContext>());
            await sellHandler.HandleAsync(sellCommand);
        }

        var command = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Canceled
        };
        command.SetId(product.Id);

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductUpdateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Status.Should().Be(ProductStatus.Canceled.ToString());
        result.CancelDate.Should().NotBeNull();
    }

    [Fact]
    public async Task ProductUpdateStatusCommand_WithNonExistentProduct_ShouldThrowException()
    {
        var command = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Sold
        };
        command.SetId(Guid.NewGuid());

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductUpdateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task ProductUpdateStatusCommand_ReturnNonSoldProduct_ShouldThrowDomainException()
    {
        var product = await CreateTestProduct();

        var command = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Returned
        };
        command.SetId(product.Id);

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductUpdateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<DomainException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task ProductUpdateStatusCommand_SellAlreadySoldProduct_ShouldThrowDomainException()
    {
        var product = await CreateTestProduct();
        
        var sellCommand = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Sold
        };
        sellCommand.SetId(product.Id);
        
        using (var sellScope = ServiceProvider.CreateScope())
        {
            var sellHandler = new ProductUpdateCommandHandler(
                sellScope.ServiceProvider.GetRequiredService<IApplicationDbContext>());
            await sellHandler.HandleAsync(sellCommand);
        }

        var command = new ProductUpdateStatusCommand
        {
            Status = ProductStatus.Sold
        };
        command.SetId(product.Id);

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductUpdateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<DomainException>(
            async () => await handler.HandleAsync(command));
    }
}
