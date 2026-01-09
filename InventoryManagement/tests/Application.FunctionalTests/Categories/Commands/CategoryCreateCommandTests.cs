using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Categories.Commands;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Categories.Commands;

public class CategoryCreateCommandTests : TestingBase
{
    [Fact]
    public async Task CategoryCreateCommand_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var command = new CategoryCreateCommand
        {
            Name = "Electronics",
            Shortcode = "ELEC"
        };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Electronics");
        result.Shortcode.Should().Be("ELEC");
        result.ParentId.Should().BeNull();

        var savedCategory = await ExecuteDbContextAsync(async context =>
            await context.Categories.FirstOrDefaultAsync(c => c.Id == result.Id));

        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task CategoryCreateCommand_WithParent_ShouldCreateCategoryWithParent()
    {
        // Arrange
        var parentCategory = new Category("Electronics", "ELEC");

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.Add(parentCategory);
            await context.SaveChangesAsync();
        });

        var command = new CategoryCreateCommand
        {
            Name = "Smartphones",
            Shortcode = "PHONE",
            ParentId = parentCategory.Id
        };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Smartphones");
        result.ParentId.Should().Be(parentCategory.Id);
        result.ParentName.Should().Be("Electronics");
    }

    [Fact]
    public async Task CategoryCreateCommand_WithInvalidParentId_ShouldThrowException()
    {
        // Arrange
        var command = new CategoryCreateCommand
        {
            Name = "Smartphones",
            Shortcode = "PHONE",
            ParentId = Guid.NewGuid() // Non-existent parent
        };

        // Act & Assert
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryCreateCommandHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(
            async () => await handler.HandleAsync(command));
    }

    [Fact]
    public async Task CategoryCreateCommand_WithEmptyName_ShouldFailValidation()
    {
        // Arrange
        var command = new CategoryCreateCommand
        {
            Name = "",
            Shortcode = "TEST"
        };

        using var scope = ServiceProvider.CreateScope();
        var validator = new CategoryCreateCommandValidator(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public async Task CategoryCreateCommand_WithEmptyShortcode_ShouldFailValidation()
    {
        // Arrange
        var command = new CategoryCreateCommand
        {
            Name = "Electronics",
            Shortcode = ""
        };

        using var scope = ServiceProvider.CreateScope();
        var validator = new CategoryCreateCommandValidator(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Shortcode));
    }
}
