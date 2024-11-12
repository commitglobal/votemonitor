using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.TestUtils;
using Vote.Monitor.TestUtils.Utils;

namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class UpdateValidatorTests
{
    private readonly Update.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new Update.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new Update.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Validation_ShouldFail_When_Id_Empty()
    {
        // Arrange
        var request = new Update.Request { Id = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validation_ShouldFail_When_Status_Empty()
    {
        // Arrange
        var request = new Update.Request();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }


    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Tags_Contain_Empty(string emptyTag)
    {
        // Arrange
        var request = new Update.Request
        {
            Tags = [
                "a tag",
                emptyTag
            ]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_FirstName_Empty(string firstName)
    {
        // Arrange
        var request = new Update.Request { FirstName = firstName };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Fact]
    public void Validation_ShouldFail_When_FirstName_Exceeds_Limit()
    {
        // Arrange
        var request = new Update.Request { FirstName = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_LastName_Empty(string phoneNumber)
    {
        // Arrange
        var request = new Update.Request { LastName = phoneNumber };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Fact]
    public void Validation_ShouldFail_When_LastName_Exceeds_Limit()
    {
        // Arrange
        var request = new Update.Request { LastName = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_PhoneNumber_Empty(string phoneNumber)
    {
        // Arrange
        var request = new Update.Request { PhoneNumber = phoneNumber };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validation_ShouldFail_When_PhoneNumber_Exceeds_Limit()
    {
        // Arrange
        var request = new Update.Request { PhoneNumber = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidRequest()
    {
        // Arrange
        var request = new Update.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "07942069",
            Status = MonitoringObserverStatus.Active,
            Tags = []
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
