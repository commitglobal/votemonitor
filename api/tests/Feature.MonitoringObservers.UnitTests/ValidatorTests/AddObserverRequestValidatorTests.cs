using Feature.MonitoringObservers.Add;
using Feature.MonitoringObservers.Parser;

namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class AddObserverRequestValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Observers =
            [
                new MonitoringObserverImportModel
                {
                    Email = "test@test.com",
                    Tags = ["tag1", "tag2"],
                    FirstName = "firstName",
                    LastName = "lastName",
                    PhoneNumber = "phoneNumber"
                }
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
