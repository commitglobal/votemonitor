namespace Feature.CitizenReports.Attachments.UnitTests.ValidatorTests;

public class GetRequestValidatorTests
{
    private readonly Get.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Get.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }


    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Get.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_CitizenReportId_Empty()
    {
        // Arrange
        var request = new Get.Request { CitizenReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CitizenReportId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            CitizenReportId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}