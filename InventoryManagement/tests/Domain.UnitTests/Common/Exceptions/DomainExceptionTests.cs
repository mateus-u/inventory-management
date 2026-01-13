using Domain.Common.Exceptions;

namespace Domain.UnitTests.Common.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void DomainException_Constructor_ShouldSetMessage()
    {
        // Arrange
        var expectedMessage = "Test error message";

        // Act
        var exception = new DomainException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void DomainException_ShouldInheritFromException()
    {
        // Arrange
        var exception = new DomainException("Test");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void DomainException_CanBeCaughtAsException()
    {
        // Arrange
        var expectedMessage = "Domain error";
        Exception? caughtException = null;

        // Act
        try
        {
            throw new DomainException(expectedMessage);
        }
        catch (Exception ex)
        {
            caughtException = ex;
        }

        // Assert
        Assert.NotNull(caughtException);
        Assert.IsType<DomainException>(caughtException);
        Assert.Equal(expectedMessage, caughtException.Message);
    }

    [Fact]
    public void DomainException_WithEmptyMessage_ShouldAcceptEmptyString()
    {
        // Act
        var exception = new DomainException(string.Empty);

        // Assert
        Assert.Equal(string.Empty, exception.Message);
    }

    [Theory]
    [InlineData("Simple error")]
    [InlineData("Error with special characters: !@#$%")]
    [InlineData("Error with numbers: 123456")]
    [InlineData("Very long error message that contains a lot of text to test if the exception can handle longer messages properly without any issues")]
    public void DomainException_WithVariousMessages_ShouldSetMessageCorrectly(string message)
    {
        // Act
        var exception = new DomainException(message);

        // Assert
        Assert.Equal(message, exception.Message);
    }
}
