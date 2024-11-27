using Vote.Monitor.TestUtils;

namespace Feature.MonitoringObservers.UnitTests.ValidatorTests;

public class ListValidatorTests
{
    private readonly List.Validator _validator = new();

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundId_Empty()
    {
        // Arrange
        var request = new List.Request { ElectionRoundId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Fact]
    public void Validation_ShouldFail_When_NgoId_Empty()
    {
        // Arrange
        var request = new List.Request { NgoId = Guid.Empty };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }


    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Tags_Contain_Empty(string emptyTag)
    {
        // Arrange
        var request = new List.Request
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
    [MemberData(nameof(TagsFilters))]
    public void Validation_ShouldPass_When_ValidRequest(string[]? tags)
    {
        // Arrange
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            Tags = tags
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> TagsFilters =>
        new List<object[]>
        {
            new object[] { null },
            new object[] {  new []{ "a tag" } }
        };
}
