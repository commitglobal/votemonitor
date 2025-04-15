using Feature.PollingStations.Create;

namespace Feature.PollingStations.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundIdEmpty()
    {
        // Arrange
        var request = new Create.Request { ElectionRoundId = Guid.Empty, };
        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_WhenNullPollingStations()
    {
        // Arrange
        var request = new Create.Request { ElectionRoundId = Guid.NewGuid(), PollingStations = null! };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PollingStations);
    }

    [Fact]
    public void Validation_ShouldFail_WhenEmptyPollingStations()
    {
        // Arrange
        var request = new Create.Request { ElectionRoundId = Guid.NewGuid(), PollingStations = [] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PollingStations);
    }

    [Fact]
    public void Validation_ShouldFail_WhenInvalidPollingStation()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStations =
            [
                new Request.PollingStationRequest
                {
                    Level1 = "Level1", Number = "1", Address = "some address", DisplayOrder = 1
                },
                new Request.PollingStationRequest
                {
                    Level1 = "",
                    Level2 = "Level 2",
                    Number = "2",
                    Address = "some other address",
                    DisplayOrder = 2
                },
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PollingStations);
    }

    [Fact]
    public void Validation_ShouldPass_WhenValidRequest()
    {
        // Arrange
        var request = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            PollingStations =
            [
                new Request.PollingStationRequest
                {
                    Level1 = "Level1", Number = "1", Address = "some address", DisplayOrder = 1
                }
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
