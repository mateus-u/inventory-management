using Domain.Enums;

namespace Domain.UnitTests.Enums;

public class ProductStatusTests
{
    [Fact]
    public void ProductStatus_Created_ShouldHaveValue1()
    {
        var status = ProductStatus.Created;

        Assert.Equal(1, (int)status);
    }

    [Fact]
    public void ProductStatus_Sold_ShouldHaveValue2()
    {
        var status = ProductStatus.Sold;

        Assert.Equal(2, (int)status);
    }

    [Fact]
    public void ProductStatus_Canceled_ShouldHaveValue3()
    {
        var status = ProductStatus.Canceled;

        Assert.Equal(3, (int)status);
    }

    [Fact]
    public void ProductStatus_Returned_ShouldHaveValue4()
    {
        var status = ProductStatus.Returned;

        Assert.Equal(4, (int)status);
    }

    [Fact]
    public void ProductStatus_ShouldHaveExactlyFourValues()
    {
        var values = Enum.GetValues<ProductStatus>();

        Assert.Equal(4, values.Length);
    }

    [Fact]
    public void ProductStatus_AllValuesShouldBeUnique()
    {
        var values = Enum.GetValues<ProductStatus>();
        var intValues = values.Select(v => (int)v).ToList();

        Assert.Equal(intValues.Count, intValues.Distinct().Count());
    }

    [Theory]
    [InlineData(1, ProductStatus.Created)]
    [InlineData(2, ProductStatus.Sold)]
    [InlineData(3, ProductStatus.Canceled)]
    [InlineData(4, ProductStatus.Returned)]
    public void ProductStatus_CanBeConvertedFromInt(int value, ProductStatus expected)
    {
        var status = (ProductStatus)value;

        Assert.Equal(expected, status);
    }

    [Theory]
    [InlineData(ProductStatus.Created, "Created")]
    [InlineData(ProductStatus.Sold, "Sold")]
    [InlineData(ProductStatus.Canceled, "Canceled")]
    [InlineData(ProductStatus.Returned, "Returned")]
    public void ProductStatus_ToString_ShouldReturnCorrectName(ProductStatus status, string expectedName)
    {
        var name = status.ToString();

        Assert.Equal(expectedName, name);
    }

    [Theory]
    [InlineData("Created", ProductStatus.Created)]
    [InlineData("Sold", ProductStatus.Sold)]
    [InlineData("Canceled", ProductStatus.Canceled)]
    [InlineData("Returned", ProductStatus.Returned)]
    public void ProductStatus_CanBeParsedFromString(string name, ProductStatus expected)
    {
        var status = Enum.Parse<ProductStatus>(name);

        Assert.Equal(expected, status);
    }

    [Fact]
    public void ProductStatus_IsDefined_ShouldReturnTrueForValidValues()
    {
        Assert.True(Enum.IsDefined(ProductStatus.Created));
        Assert.True(Enum.IsDefined(ProductStatus.Sold));
        Assert.True(Enum.IsDefined(ProductStatus.Canceled));
        Assert.True(Enum.IsDefined(ProductStatus.Returned));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(99)]
    [InlineData(-1)]
    public void ProductStatus_IsDefined_ShouldReturnFalseForInvalidValues(int value)
    {
        var status = (ProductStatus)value;

        Assert.False(Enum.IsDefined(status));
    }

    [Fact]
    public void ProductStatus_GetNames_ShouldReturnAllNames()
    {
        var names = Enum.GetNames<ProductStatus>();

        Assert.Equal(4, names.Length);
        Assert.Contains("Created", names);
        Assert.Contains("Sold", names);
        Assert.Contains("Canceled", names);
        Assert.Contains("Returned", names);
    }

    [Fact]
    public void ProductStatus_Comparison_ShouldWorkCorrectly()
    {
        Assert.True(ProductStatus.Created < ProductStatus.Sold);
        Assert.True(ProductStatus.Sold < ProductStatus.Canceled);
        Assert.True(ProductStatus.Canceled < ProductStatus.Returned);
        Assert.True(ProductStatus.Created != ProductStatus.Sold);
        Assert.Equal(ProductStatus.Sold, ProductStatus.Sold);
    }

    [Fact]
    public void ProductStatus_CanBeUsedInSwitchStatement()
    {
        var status = ProductStatus.Sold;
        string result;

        result = status switch
        {
            ProductStatus.Created => "Created",
            ProductStatus.Sold => "Sold",
            ProductStatus.Canceled => "Canceled",
            ProductStatus.Returned => "Returned",
            _ => "Unknown"
        };

        Assert.Equal("Sold", result);
    }
}
