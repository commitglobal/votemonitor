namespace Feature.Locations.UnitTests.ValidatorTests;

public class UpdateRequestValidatorTests
{
    private readonly Update.Validator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var request = new Update.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Empty()
    {
        // Arrange
        var request = new Update.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Level2_Is_Empty_And_Level3_Is_Empty()
    {
        // Arrange
        var request = new Update.Request { Level2 = string.Empty, Level3 = string.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Level2);
    }

    [Fact]
    public void Should_Have_Error_When_Level2_Is_Empty_And_Level3_Is_Not_Empty()
    {
        // Arrange
        var request = new Update.Request { Level2 = string.Empty, Level3 = "Non-empty" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level2);
    }

    [Fact]
    public void Should_Have_Error_When_Level3_Is_Empty_And_Level4_Is_Not_Empty()
    {
        // Arrange
        var request = new Update.Request { Level3 = string.Empty, Level4 = "Non-empty" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level3);
    }

    [Fact]
    public void Should_Have_Error_When_Level4_Is_Empty_And_Level5_Is_Not_Empty()
    {
        // Arrange
        var request = new Update.Request { Level4 = string.Empty, Level5 = "Non-empty" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level4);
    }

    [Fact]
    public void Should_Have_Error_When_DisplayOrder_Is_Negative()
    {
        // Arrange
        var request = new Update.Request { DisplayOrder = -1 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Should_Not_Have_Error_When_DisplayOrder_Is_Zero_Or_Greater()
    {
        // Arrange
        var request = new Update.Request { DisplayOrder = 0 };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.DisplayOrder);
    }

    [Fact]
    public void Should_Have_Error_When_Tags_Has_Empty_Keys()
    {
        // Arrange
        var request = new Update.Request
        {
            Tags = new Dictionary<string, string> { { string.Empty, "Value" } }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Tags_Are_Valid()
    {
        // Arrange
        var request = new Update.Request
        {
            Tags = new Dictionary<string, string> { { "ValidKey", "Value" } }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }
}