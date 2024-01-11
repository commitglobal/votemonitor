using Vote.Monitor.Api.Feature.Observer.Services;

namespace Vote.Monitor.Api.Feature.Observer.UnitTests.Services;

public class ObserverImportModelValidatorTests
{
    private ObserverImportModel model = new ObserverImportModel
    {
        Name = "jhon",
        Email = "test@code.com",
        PhoneNumber = "12345678"
    };

    [Fact]
    public void Validate_WithValidModel_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
        result.ShouldNotHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validate_WithInvalidName_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        model.Name = "Jo"; // less than the required minimum length

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorMessage("The length of 'Name' must be at least 3 characters. You entered 2 characters.");
    }

    [Fact]
    public void Validate_WithInvalidEmail_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        model.Email = "invalidemail";


        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("'Email' is not a valid email address.");
    }

    [Fact]
    public void Validate_WithInvalidPhoneNumber_ShouldHaveValidationError()
    {
        // Arrange
        var validator = new ObserverImportModelValidator();
        model.PhoneNumber = "1234"; // less than the required minimum length

        // Act
        var result = validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithErrorMessage("The length of 'Phone Number' must be at least 8 characters. You entered 4 characters.");
    }
}
