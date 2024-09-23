namespace Feature.Citizen.Guides.UnitTests.Validators;

public class ListValidatorTests
{
    private readonly List.Validator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var model = new List.Request { ElectionRoundId = Guid.Empty };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }


    [Fact]
    public void Should_Not_Have_Error_When_Valid_Request()
    {
        // Arrange
        var model = new List.Request
        {
            ElectionRoundId = Guid.NewGuid()
        };
        
        // Act
        var result = _validator.TestValidate(model);
        
        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}