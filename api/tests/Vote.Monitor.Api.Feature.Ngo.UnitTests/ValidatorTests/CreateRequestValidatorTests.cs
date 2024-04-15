using Vote.Monitor.Api.Feature.Ngo.Create;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Api.Feature.Ngo.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Validator _validator = new();

    [Fact]
    public void Validation_ShouldPass_When_Name_NotEmpty()
    {
        // Arrange
        var request = new Request { Name = Guid.NewGuid().ToString() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Name_Empty(string name)
    {
        // Arrange
        var request = new Request { Name = name };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validation_ShouldFail_When_Name_Exceeds_Limit()
    {
        // Arrange
        var request = new Request { Name = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
    [Fact]
    public void Validation_ShouldFail_When_Name_Below_Limit()
    {
        // Arrange
        var request = new Request { Name = "aa" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
