﻿namespace Feature.ElectionRounds.UnitTests.ValidatorTests;

public class UnarchiveRequestValidatorTests
{
    private readonly Unarchive.Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_Id_NotEmpty()
    {
        // Arrange
        var request = new Unarchive.Request { Id = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Unarchive.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
