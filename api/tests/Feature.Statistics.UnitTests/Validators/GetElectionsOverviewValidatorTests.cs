using FastEndpoints;
using Feature.Statistics.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Feature.Statistics.UnitTests.Validators;

public class GetElectionsOverviewValidatorTests
{
    private const string API_KEY = "secret-api-key";

    private readonly GetElectionsOverview.Validator _validator = Factory.CreateValidator<GetElectionsOverview.Validator>(
        sp =>
        {
            sp.Configure<StatisticsFeatureOptions>(x =>
            {
                x.ApiKey = API_KEY;
            });
        });

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundIds_Empty()
    {
        // Arrange
        var request = new GetElectionsOverview.Request { ElectionRoundIds = [] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundIds);
    }

    [Fact]
    public void Validation_ShouldFail_When_ElectionRoundIds_Contains_EmptyId()
    {
        // Arrange
        var request = new GetElectionsOverview.Request { ElectionRoundIds = [Guid.NewGuid(), Guid.Empty] };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundIds);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_ApiKey_Empty(string emptyValue)
    {
        // Arrange
        var request = new GetElectionsOverview.Request { ApiKey = emptyValue};

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApiKey);
    }

    [Fact]
    public void Validation_ShouldFail_When_ApiKey_Invalid()
    {
        // Arrange
        var request = new GetElectionsOverview.Request { ApiKey = "invalid-value"};

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ApiKey);
    }

    [Fact]
    public void Validation_ShouldPass_When_Request_Valid()
    {
        // Arrange
        var request = new GetElectionsOverview.Request
        {
            ApiKey = API_KEY,
            ElectionRoundIds = [Guid.NewGuid()]
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
