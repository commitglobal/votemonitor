﻿using Vote.Monitor.Api.Feature.Observer.Parser;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Services;

public class ObserverImportModelValidatorTests
{
    private ObserverImportModel _model = new()
    {
        FirstName = "jhon",
        LastName = "Smith",
        Email = "test@code.com",
        PhoneNumber = "12345678",
        Password = "Passw0rd"
    };

    [Fact]
    public void Validate_WithValidModel_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();

        // Act
        var result = validator.TestValidate(_model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidFirstName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        _model = _model with { FirstName = "" }; // less than the required minimum length

        // Act
        var result = validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
    [Fact]
    public void Validate_WithInvalidLastName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        _model = _model with { LastName = "" }; // less than the required minimum length

        // Act
        var result = validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        _model = _model with { Email = "invalidemail" };

        // Act
        var result = validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_WithInvalidPhoneNumber_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        _model = _model with { PhoneNumber = "12" }; // less than the required minimum length

        // Act
        var result = validator.TestValidate(_model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }
}
