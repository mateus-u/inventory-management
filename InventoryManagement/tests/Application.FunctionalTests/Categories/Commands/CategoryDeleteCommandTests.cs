using Application.Common.Interfaces;
using Application.UseCases.Categories.Commands;
using Application.UseCases.Products.Commands;
using Domain.Entities;
using Domain.ValueObjects;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Categories.Commands;

public class CategoryDeleteCommandTests : TestingBase
{
    [Fact]
    public async Task CategoryDeleteCommand_WithValidId_ShouldDeleteCategory()
    {
        var category = new Category("Electronics", "ELEC");

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        });

        var command = new CategoryDeleteCommand { Id = category.Id };

        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryDeleteCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        result.Should().BeTrue();

        var deletedCategory = await ExecuteDbContextAsync(async context =>
            await context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id));

        deletedCategory.Should().BeNull();
    }

    [Fact]
    public async Task CategoryDeleteCommand_WithNonExistentId_ShouldReturnFalse()
    {
        var command = new CategoryDeleteCommand { Id = Guid.NewGuid() };

        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryDeleteCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task CategoryDeleteCommand_WithAssociatedProducts_ShouldThrowValidationException()
    {
        var category = new Category("Electronics", "ELEC");
        var supplier = new Supplier(
            "Test Supplier",
            new Email("supplier@test.com"),
            Currency.FromCode("USD"),
            Country.FromCode("US"));

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.Add(category);
            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
        });

        var productCommand = new ProductCreateCommand
        {
            Description = "Test Product",
            AcquisitionCost = 100m,
            AcquisitionCostUSD = 100m,
            SupplierId = supplier.Id,
            CategoryId = category.Id
        };

        using (var scope = ServiceProvider.CreateScope())
        {
            var productHandler = new ProductCreateCommandHandler(
                scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());
            await productHandler.HandleAsync(productCommand);
        }

        var command = new CategoryDeleteCommand { Id = category.Id };

        using var deleteScope = ServiceProvider.CreateScope();
        var handler = new CategoryDeleteCommandHandler(
            deleteScope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<ValidationException>(
            async () => await handler.HandleAsync(command));

        var categoryStillExists = await ExecuteDbContextAsync(async context =>
            await context.Categories.AnyAsync(c => c.Id == category.Id));

        categoryStillExists.Should().BeTrue();
    }

    [Fact]
    public async Task CategoryDeleteCommand_WithEmptyId_ShouldFailValidation()
    {
        var command = new CategoryDeleteCommand { Id = Guid.Empty };

        using var scope = ServiceProvider.CreateScope();
        var validator = new CategoryDeleteCommandValidator();

        var result = await validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }
}
