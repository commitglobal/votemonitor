using FluentValidation.TestHelper;
using Vote.Monitor.Domain.Entities.CitizenGuideAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;
using Vote.Monitor.TestUtils.Fakes;

namespace Feature.ObserverGuide.UnitTests.Validators;

public class CreateValidatorTests
{
    private readonly Create.Validator _validator = new();

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var model = new Create.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var model = new Create.Request { Title = string.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var model = new Create.Request { Title = new string('A', 257) };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Have_Error_When_Attachment_Is_Empty_For_Document_Type()
    {
        // Arrange
        var model = new Create.Request { GuideType = ObserverGuideType.Document, Attachment = null };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attachment);
    }

    [Fact]
    public void Should_Have_Error_When_Attachment_Exceeds_File_Size_Limit_For_Document_Type()
    {
        // Arrange
        var model = new Create.Request
        {
            GuideType = ObserverGuideType.Document,
            Attachment = FakeFormFile.New().HavingLength(51 * 1024 * 1024).Please() // 51 MB file
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Attachment);
    }

    [Fact]
    public void Should_Have_Error_When_WebsiteUrl_Is_Empty_For_Website_Type()
    {
        // Arrange
        var model = new Create.Request { GuideType = ObserverGuideType.Website, WebsiteUrl = string.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    [Fact]
    public void Should_Have_Error_When_WebsiteUrl_Exceeds_Maximum_Length_For_Website_Type()
    {
        // Arrange
        var model = new Create.Request
        {
            GuideType = ObserverGuideType.Website,
            WebsiteUrl = new string('A', 2049)
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    [Fact]
    public void Should_Have_Error_When_WebsiteUrl_Is_Invalid_For_Website_Type()
    {
        // Arrange
        var model = new Create.Request
        {
            GuideType = ObserverGuideType.Website,
            WebsiteUrl = "invalid-url"
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WebsiteUrl);
    }

    [Fact]
    public void Should_Have_Error_When_Text_Is_Empty_For_Text_Type()
    {
        // Arrange
        var model = new Create.Request { GuideType = ObserverGuideType.Text, Text = string.Empty };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Text);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Request()
    {
        // Arrange
        var model = new Create.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Title = "Valid Title",
            GuideType = ObserverGuideType.Document,
            Attachment = FakeFormFile.New().Please()
        };

        // Act
        var result = _validator.TestValidate(model);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}