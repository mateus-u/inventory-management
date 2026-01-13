using Domain.Common.Exceptions;
using Domain.Entities;

namespace Domain.UnitTests.Entities;

public class CategoryTests
{
    [Fact]
    public void Category_Constructor_ShouldCreateCategoryWithCorrectProperties()
    {
        var name = "Electronics";
        var shortcode = "ELEC";

        var category = new Category(name, shortcode);

        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(shortcode, category.Shortcode);
        Assert.Null(category.Parent);
    }

    [Fact]
    public void Category_Constructor_WithParent_ShouldSetParentCorrectly()
    {
        var parentCategory = new Category("Electronics", "ELEC");
        var name = "Smartphones";
        var shortcode = "PHONE";

        var category = new Category(name, shortcode, parentCategory);

        Assert.NotEqual(Guid.Empty, category.Id);
        Assert.Equal(name, category.Name);
        Assert.Equal(shortcode, category.Shortcode);
        Assert.NotNull(category.Parent);
        Assert.Equal(parentCategory, category.Parent);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Category_WithEmptyOrWhiteSpaceName_ShouldThrowDomainException(string? invalidName)
    {
        var shortcode = "TEST";

        var exception = Assert.Throws<DomainException>(() => new Category(invalidName!, shortcode));
        Assert.Contains("Name cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Category_WithEmptyOrWhiteSpaceShortcode_ShouldThrowDomainException(string? invalidShortcode)
    {
        var name = "Test Category";

        var exception = Assert.Throws<DomainException>(() => new Category(name, invalidShortcode!));
        Assert.Contains("Shortcode cannot be empty", exception.Message);
    }

    [Theory]
    [InlineData("Electronics", "ELEC")]
    [InlineData("Clothing", "CLOTH")]
    [InlineData("Home & Garden", "HOME")]
    [InlineData("Sports & Outdoors", "SPORT")]
    public void Category_WithVariousValidData_ShouldCreateSuccessfully(string name, string shortcode)
    {
        var category = new Category(name, shortcode);

        Assert.NotNull(category);
        Assert.Equal(name, category.Name);
        Assert.Equal(shortcode, category.Shortcode);
    }

    [Fact]
    public void Category_HierarchyWithMultipleLevels_ShouldWorkCorrectly()
    {
        var rootCategory = new Category("Electronics", "ELEC");
        var midCategory = new Category("Mobile Devices", "MOBILE", rootCategory);
        var leafCategory = new Category("Smartphones", "PHONE", midCategory);

        Assert.Null(rootCategory.Parent);
        Assert.Equal(rootCategory, midCategory.Parent);
        Assert.Equal(midCategory, leafCategory.Parent);
    }
}
