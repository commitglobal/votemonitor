﻿namespace Vote.Monitor.Api.Feature.Monitoring.UnitTests.ValidatorTests;

public class RemoveNgoRequestValidatorTests
{
    private readonly RemoveNgo.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_ElectionRoundId_NotEmpty()
    {
        // Arrange
        var request = new RemoveNgo.Request { ElectionRoundId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new RemoveNgo.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldPass_When_NgoId_NotEmpty()
    {
        // Arrange
        var request = new RemoveNgo.Request { NgoId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new RemoveNgo.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }
}
