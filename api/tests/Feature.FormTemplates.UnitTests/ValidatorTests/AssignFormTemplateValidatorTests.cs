using Feature.FormTemplates.AssignTemplates;
using NPOI.SS.Formula.Functions;

namespace Feature.FormTemplates.UnitTests.ValidatorTests;

public class AssignFormTemplateValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_Should_Pass_When_Valid_Request()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        List<Guid> formTemplateIds = [Guid.NewGuid(), Guid.NewGuid()];
        var request = new Request { ElectionRoundId = electionRoundId, FormTemplateIds = formTemplateIds };
        
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ElectionRoundId);
        result.ShouldNotHaveValidationErrorFor(x => x.FormTemplateIds);
    }

    [Fact]
    public void Validation_Should_Fail_When_ElectionRoundId_is_Empty()
    {
        // Arrange
        var electionRoundId = Guid.Empty;
        var request = new Request { ElectionRoundId = electionRoundId };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_Should_Fail_When_FormTemplateIds_is_Empty()
    {
        // Arrange
        List<Guid> formTemplateIds = [];
        var request = new Request { FormTemplateIds = formTemplateIds };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FormTemplateIds);
    }
}
