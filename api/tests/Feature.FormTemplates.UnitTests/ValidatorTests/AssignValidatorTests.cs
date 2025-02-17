using Feature.FormTemplates.AssignTemplates;

namespace Feature.FormTemplates.UnitTests.ValidatorTests;

public class AssignValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_Should_Pass_When_ElectionRoundId_and_FormTemplateIds_are_NotEmpty()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            FormTemplateIds = [Guid.NewGuid(), Guid.NewGuid()]
        };
        
        // Act
        
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ElectionRoundId);
        result.ShouldNotHaveValidationErrorFor(x => x.FormTemplateIds);
    }
}
