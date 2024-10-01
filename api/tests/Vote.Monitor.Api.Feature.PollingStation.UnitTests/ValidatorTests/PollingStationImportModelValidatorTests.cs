using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.PollingStation.UnitTests.ValidatorTests;

public class PollingStationImportModelValidatorTests
{
    private readonly PollingStationImportModelValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Validation_ShouldPass_When_DisplayOrder_GreaterThanOrEqualToZero(int displayOrder)
    {
        // Arrange
        var importModel = new PollingStationImportModel
        {
            DisplayOrder = displayOrder,
            Address = "123 Main St",
            Tags = new List<TagImportModel>
            {
               new() { Name = "Tag1" ,Value = "value1"},
               new() { Name = "Tag2" ,Value =  "value2"}
            }
        };

        var context = new ValidationContext<PollingStationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x=>x.DisplayOrder);
    }

    [Fact]
    public void Validation_ShouldFail_When_DisplayOrder_LessThanZero()
    {
        // Arrange
        var importModel = new PollingStationImportModel
        {
            DisplayOrder = -1,
            Address = "123 Main St",
            Tags = new List<TagImportModel>
            {
                new() { Name = "Tag1" ,Value = "value1"},
                new() { Name = "Tag2" ,Value =  "value2"}
            }
        };
        var context = new ValidationContext<PollingStationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DisplayOrder)
            .WithErrorMessage("Polling station on row 1 has invalid Display Order. Display Order should be greater than 0.");
    }

    [Fact]
    public void Validation_ShouldPass_When_Address_NotEmpty()
    {
        // Arrange
        var importModel = new PollingStationImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new List<TagImportModel>
            {
                new() { Name = "Tag1" ,Value = "value1"},
                new() { Name = "Tag2" ,Value =  "value2"}
            }
        };
        var context = new ValidationContext<PollingStationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Address_Empty(string address)
    {
        // Arrange
        var importModel = new PollingStationImportModel
        {
            DisplayOrder = 5,
            Address = address,
            Tags = new List<TagImportModel>
            {
                new() { Name = "Tag1" ,Value = "value1"},
                new() { Name = "Tag2" ,Value =  "value2"}
            }
        };
        var context = new ValidationContext<PollingStationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Address)
            .WithErrorMessage("Polling station on row 1 has invalid Address. Address should be not be empty.");
    }

    [Fact]
    public void Validation_ShouldPass_When_Tags_NotEmpty()
    {
        // Arrange
        var importModel = new PollingStationImportModel
        {
            DisplayOrder = 5,
            Address = "123 Main St",
            Tags = new List<TagImportModel>
            {
                new() { Name = "Tag1" ,Value = "value1"},
                new() { Name = "Tag2" ,Value =  "value2"}
            }
        };
        var context = new ValidationContext<PollingStationImportModel>(importModel);
        context.RootContextData["RowIndex"] = 1;

        // Act
        var result = _validator.TestValidate(context);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Tags);
    }

    //[Fact]
    //public void Validation_ShouldFail_When_Tags_Empty()
    //{
    //    // Arrange
    //    var importModel = new PollingStationImportModel
    //    {
    //        DisplayOrder = 5,
    //        Address = "123 Main St",
    //        Tags = new()
    //    };
    //    var context = new ValidationContext<PollingStationImportModel>(importModel);
    //    context.RootContextData["RowIndex"] = 1;

    //    // Act
    //    var result = _validator.TestValidate(context);

    //    // Assert
    //    result
    //        .ShouldHaveValidationErrorFor(x => x.Tags)
    //        .WithErrorMessage("Polling station on row 1 has invalid Tags. At least one value for Tags is required.");
    //}

    //[Fact]
    //public void Validation_ShouldFail_When_Tags_Null()
    //{
    //    // Arrange
    //    var importModel = new PollingStationImportModel
    //    {
    //        DisplayOrder = 5,
    //        Address = "123 Main St",
    //        Tags = null
    //    };
    //    var context = new ValidationContext<PollingStationImportModel>(importModel);
    //    context.RootContextData["RowIndex"] = 1;

    //    // Act
    //    var result = _validator.TestValidate(context);

    //    // Assert
    //    result
    //        .ShouldHaveValidationErrorFor(x => x.Tags)
    //        .WithErrorMessage("Polling station on row 1 has invalid Tags. At least one value for Tags is required.");
    //}

    //[Theory]
    //[MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    //public void Validation_ShouldFail_When_Tags_WithEmptyKey(string key)
    //{
    //    // Arrange
    //    var importModel = new PollingStationImportModel
    //    {
    //        DisplayOrder = 5,
    //        Address = "123 Main St",
    //        Tags = new List<TagImportModel>
    //        {
    //            new () { Name = key, Value= "Value" }
    //        }
    //    };
    //    var context = new ValidationContext<PollingStationImportModel>(importModel);
    //    context.RootContextData["RowIndex"] = 1;

    //    // Act
    //    var result = _validator.TestValidate(context);

    //    // Assert
    //    result.ShouldHaveValidationErrorFor(x => x.Tags)
    //        .WithErrorMessage("Polling station on row 1 has invalid Tags. Tag name is empty.");
    //}   
    
    //[Fact]
    //public void Validation_ShouldFail_When_DuplicatedTags()
    //{
    //    // Arrange
    //    var importModel = new PollingStationImportModel
    //    {
    //        DisplayOrder = 5,
    //        Address = "123 Main St",
    //        Tags = new List<TagImportModel>
    //        {
    //            new () { Name = "Tag", Value= "Value" },
    //            new () { Name = "Tag", Value= "Value" }
    //        }
    //    };
    //    var context = new ValidationContext<PollingStationImportModel>(importModel);
    //    context.RootContextData["RowIndex"] = 1;

    //    // Act
    //    var result = _validator.TestValidate(context);

    //    // Assert
    //    result.ShouldHaveValidationErrorFor(x => x.Tags)
    //        .WithErrorMessage("Polling station on row 1 has invalid Tags. Duplicated tag name found 'Tag'.");
    //}
}
