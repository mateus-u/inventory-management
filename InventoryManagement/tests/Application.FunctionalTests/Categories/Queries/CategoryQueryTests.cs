using Application.Common.Interfaces;
using Application.FunctionalTests;
using Application.UseCases.Categories.Queries;
using Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Application.FunctionalTests.Categories.Queries;

public class CategoryQueryTests : TestingBase
{
    [Fact]
    public async Task CategoryGetAllQuery_WithNoCategories_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new CategoryGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CategoryGetAllQuery_WithCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var category1 = new Category("Electronics", "ELEC");
        var category2 = new Category("Clothing", "CLOTH");
        var category3 = new Category("Home", "HOME");

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();
        });

        var query = new CategoryGetAllQuery();

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryGetAllQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(c => c.Name == "Electronics");
        result.Should().Contain(c => c.Name == "Clothing");
        result.Should().Contain(c => c.Name == "Home");
    }

    [Fact]
    public async Task CategoryGetByIdQuery_WithExistingCategory_ShouldReturnCategory()
    {
        // Arrange
        var category = new Category("Electronics", "ELEC");

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.Add(category);
            await context.SaveChangesAsync();
        });

        var query = new CategoryGetByIdQuery { Id = category.Id };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.Name.Should().Be("Electronics");
        result.Shortcode.Should().Be("ELEC");
    }

    [Fact]
    public async Task CategoryGetByIdQuery_WithNonExistentCategory_ShouldReturnNull()
    {
        // Arrange
        var query = new CategoryGetByIdQuery { Id = Guid.NewGuid() };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CategoryGetByIdQuery_WithParentCategory_ShouldIncludeParentInfo()
    {
        // Arrange
        var parentCategory = new Category("Electronics", "ELEC");
        var childCategory = new Category("Smartphones", "PHONE", parentCategory);

        await ExecuteDbContextAsync(async context =>
        {
            context.Categories.Add(childCategory);
            await context.SaveChangesAsync();
        });

        var query = new CategoryGetByIdQuery { Id = childCategory.Id };

        // Act
        using var scope = ServiceProvider.CreateScope();
        var handler = new CategoryGetByIdQueryHandler(
            scope.ServiceProvider.GetRequiredService<IApplicationDbContext>());

        var result = await handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.ParentId.Should().Be(parentCategory.Id);
        result.ParentName.Should().Be("Electronics");
    }
}
