using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Products.Commands;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Products.Commands;

public class ProductCreateCommandTests : TestingBase
{
    [Fact]
    public async Task ProductCreateCommand_WithValidData_ShouldCreateProduct()
    {
        var supplier = new Supplier("Test Supplier", new Email("test@supplier.com"), 
            Currency.FromCode("USD"), Country.FromCode("US"));
        var category = new Category("Electronics", "ELEC");

        await ExecuteDbContextAsync(async context =>
        {
            context.Suppliers.Add(supplier);
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        });

        var command = new ProductCreateCommand
        {
            Description = "Test Product",
            SupplierId = supplier.Id,
            CategoryId = category.Id,
            AcquisitionCost = 100m,
            AcquisitionCostUSD = 100m
        };

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());
        
        var result = await handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Description.Should().Be("Test Product");
        result.SupplierId.Should().Be(supplier.Id);
        result.CategoryId.Should().Be(category.Id);
        result.Status.Should().Be(ProductStatus.Created.ToString());

        var savedProduct = await ExecuteDbContextAsync(async context =>
            await context.Products.FirstOrDefaultAsync(p => p.Id == result.Id));

        savedProduct.Should().NotBeNull();
        savedProduct!.Description.Should().Be("Test Product");
    }

    [Fact]
    public async Task ProductCreateCommand_WithInvalidSupplier_ShouldThrowException()
    {
        var category = new Category("Electronics", "ELEC");

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        });

        var command = new ProductCreateCommand
        {
            Description = "Test Product",
            SupplierId = Guid.NewGuid(),
            CategoryId = category.Id,
            AcquisitionCost = 100m,
            AcquisitionCostUSD = 100m
        };

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task ProductCreateCommand_WithInvalidCategory_ShouldThrowException()
    {
        var supplier = new Supplier("Test Supplier", new Email("test@supplier.com"),
            Currency.FromCode("USD"), Country.FromCode("US"));

        await ExecuteDbContextAsync(async context =>
        {
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
        });

        var command = new ProductCreateCommand
        {
            Description = "Test Product",
            SupplierId = supplier.Id,
            CategoryId = Guid.NewGuid(),
            AcquisitionCost = 100m,
            AcquisitionCostUSD = 100m
        };

        using var scope = ServiceProvider.CreateScope();
        var handler = new ProductCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task ProductCreateCommand_WithEmptyDescription_ShouldFailValidation()
    {
        var command = new ProductCreateCommand
        {
            Description = "",
            SupplierId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            AcquisitionCost = 100m,
            AcquisitionCostUSD = 100m
        };

        var validator = new ProductCreateCommandValidator();

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Description));
    }

    [Fact]
    public async Task ProductCreateCommand_WithTooLongDescription_ShouldFailValidation()
    {
        var command = new ProductCreateCommand
        {
            Description = new string('a', 501), // Exceeds 500 characters
            SupplierId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            AcquisitionCost = 100m,
            AcquisitionCostUSD = 100m
        };

        var validator = new ProductCreateCommandValidator();

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Description));
    }
}
