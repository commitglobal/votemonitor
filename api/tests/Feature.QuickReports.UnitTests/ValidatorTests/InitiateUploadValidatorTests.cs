namespace Feature.QuickReports.UnitTests.ValidatorTests;

public class InitiateUploadValidatorTests
{
    private readonly InitiateUpload.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ObserverId_Empty()
    {
        // Arrange
        var request = new InitiateUpload.Request { ObserverId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ObserverId);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new InitiateUpload.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }
    
    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new InitiateUpload.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_QuickReportId_Empty()
    {
        // Arrange
        var request = new InitiateUpload.Request { QuickReportId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.QuickReportId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validation_ShouldFail_When_NumberOfUploadParts_LessOrEqualToZero(int numberOfUploadParts)
    {
        // Arrange
        var request = new InitiateUpload.Request { NumberOfUploadParts = numberOfUploadParts };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NumberOfUploadParts);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_EmptyFileName(string fileName)
    {
        // Arrange
        var request = new InitiateUpload.Request { FileName = fileName };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NumberOfUploadParts);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyAndNullStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_EmptyContentType(string contentType)
    {
        // Arrange
        var request = new InitiateUpload.Request { ContentType = contentType };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NumberOfUploadParts);
    }

    [Fact]
    public void Validation_ShouldPass_When_LastUpdatedAt_Null()
    {
        // Arrange
        var request = new InitiateUpload.Request { LastUpdatedAt = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.LastUpdatedAt);
    }

    [Fact]
    public void Validation_ShouldFail_When_LastUpdatedAt_NotUtc()
    {
        // Arrange
        var request =
            new InitiateUpload.Request { LastUpdatedAt = new DateTime(2025, 04, 20, 06, 9, 00, DateTimeKind.Local) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastUpdatedAt);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new InitiateUpload.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            LastUpdatedAt = DateTime.UtcNow,
            ContentType = "image/jpeg",
            FileName = "test.jpg",
            NumberOfUploadParts = 1,
            QuickReportId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
