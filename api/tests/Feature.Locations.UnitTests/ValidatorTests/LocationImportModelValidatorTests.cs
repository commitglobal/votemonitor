namespace Feature.Locations.UnitTests.ValidatorTests;

public class LocationImportModelValidatorTests
{
    private readonly LocationImportModelValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Level2_Is_Empty_And_Level3_Is_Empty()
    {
        // Arrange
        var importModel = new LocationImportModel { Level2 = string.Empty, Level3 = string.Empty };
        var context = new ValidationContext<LocationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Level2);
    }

    [Fact]
    public void Should_Have_Error_When_Level2_Is_Empty_And_Level3_Is_Not_Empty()
    {
        // Arrange
        var importModel = new LocationImportModel { Level2 = string.Empty, Level3 = "Non-empty" };
        var context = new ValidationContext<LocationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level2);
    }

    [Fact]
    public void Should_Have_Error_When_Level3_Is_Empty_And_Level4_Is_Not_Empty()
    {
        // Arrange
        var importModel = new LocationImportModel { Level3 = string.Empty, Level4 = "Non-empty" };
        var context = new ValidationContext<LocationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level3);
    }

    [Fact]
    public void Should_Have_Error_When_Level4_Is_Empty_And_Level5_Is_Not_Empty()
    {
        // Arrange
        var importModel = new LocationImportModel { Level4 = string.Empty, Level5 = "Non-empty" };
        var context = new ValidationContext<LocationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Level4);
    }

    [Fact]
    public void Should_Have_Error_When_DisplayOrder_Is_Negative()
    {
        // Arrange
        var importModel = new LocationImportModel { DisplayOrder = -1 };
        var context = new ValidationContext<LocationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder);
    }
}