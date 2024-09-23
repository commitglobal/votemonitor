using Xunit;
using FluentValidation.TestHelper;
using Vote.Monitor.TestUtils;

namespace Feature.CitizenReports.Guides.UnitTests.Validators;

public class UpdateValidatorTests
{
    private readonly Update.Validator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var model = new Update.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Should_Have_Error_When_Title_Is_Empty(string emptyTitle)
    {
        // Arrange
        var model = new Update.Request { Title = emptyTitle };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var model = new Update.Request { Title = new string('A', 257) };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Should_Not_Have_Error_When_WebsiteUrl_Is_Empty_Or_Whitespace(string emptyUrl)
    {
        // Arrange
        var model = new Update.Request { WebsiteUrl = emptyUrl };

        // Act
        var result3 = _validator.TestValidate(model);
        result3.ShouldNotHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    [Fact]
    public void Should_Have_Error_When_WebsiteUrl_Exceeds_Maximum_Length()
    {
        // Arrange
        var model = new Update.Request { WebsiteUrl = new string('A', 2049) };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    [Fact]
    public void Should_Have_Error_When_WebsiteUrl_Is_Invalid()
    {
        // Arrange
        var model = new Update.Request { WebsiteUrl = "invalid-url" };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Request()
    {
        // Arrange
        var model = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Title = "Valid Title",
            WebsiteUrl = "https://validurl.com"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}