using FluentValidation;
using FluentValidation.TestHelper;
using Vote.Monitor.Feature.PollingStation.Import;

namespace Vote.Monitor.Feature.PollingStation.UnitTests.ValidatorTests;

public class ImportModelValidatorTests
{

    private readonly ImportModelValidator _validator = new();

    [Fact]
    public void Validate_DisplayOrder_Valid_ShouldPass()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_DisplayOrder_Invalid_ShouldFail()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = -1,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder)
            .WithErrorMessage("Polling station on row 1 has invalid Display Order. Display Order should be greater than 0.");
    }

    [Fact]
    public void Validate_Address_Valid_ShouldPass()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Fact]
    public void Validate_Address_Invalid_ShouldFail()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = String.Empty,
            Tags = new Dictionary<string, string>()
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Polling station on row 1 has invalid Address. Address should be not be empty.");
    }

    [Fact]
    public void Validate_Tags_Valid_ShouldPass()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>()
            {
                { "Tag1" ,"value1"},
                { "Tag2" ,"value2"},
            },
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public void Validate_Tags_Empty_ShouldFail()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new()
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("Polling station on row 1 has invalid Tags. At least one value for Tags is required.");
    }

    [Fact]
    public void Validate_Tags_Null_ShouldFail()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = null
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("Polling station on row 1 has invalid Tags. At least one value for Tags is required.");
    }

    [Fact]
    public void Validate_Tags_InvalidTag_ShouldFail()
    {
        // Arrange
        var importModel = new ImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new Dictionary<string, string>
            {
                { "", "Value" }
            }
        };
        var context = new ValidationContext<ImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags)
            .WithErrorMessage("Polling station on row 1 has invalid Tags. Tag key is empty.");
    }
}
