//namespace Feature.QuickReports.UnitTests.ValidatorTests;

//public class ObsoleteRequestValidatorTests
//{
//    private readonly Obsolete.Validator _validator = new();

//    [Fact]
//    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
//    {
//        // Arrange
//        var request = new Obsolete.Request { ElectionRoundId = Guid.Empty };

//        // Act
//        var result = _validator.TestValidate(request);

//        // Assert
//        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
//    }

//    [Fact]
//    public void Validation_ShouldFail_When_MonitoringNgoId_Empty()
//    {
//        // Arrange
//        var request = new Obsolete.Request { MonitoringNgoId = Guid.Empty };

//        // Act
//        var result = _validator.TestValidate(request);

//        // Assert
//        result.ShouldHaveValidationErrorFor(x => x.MonitoringNgoId);
//    }

//    [Fact]
//    public void Validation_ShouldFail_When_Id_Empty()
//    {
//        // Arrange
//        var request = new Obsolete.Request { Id = Guid.Empty };

//        // Act
//        var result = _validator.TestValidate(request);

//        // Assert
//        result.ShouldHaveValidationErrorFor(x => x.Id);
//    }

//    [Fact]
//    public void Validation_ShouldPass_When_ValidRequest()
//    {
//        // Arrange
//        var request = new Obsolete.Request
//        {
//            ElectionRoundId = Guid.NewGuid(),
//            MonitoringNgoId = Guid.NewGuid(),
//            Id = Guid.NewGuid()
//        };

//        // Act
//        var result = _validator.TestValidate(request);

//        // Assert
//        result.ShouldNotHaveAnyValidationErrors();
//    }
//}
