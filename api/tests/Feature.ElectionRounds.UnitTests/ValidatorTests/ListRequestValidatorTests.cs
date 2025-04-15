using Microsoft.Extensions.DependencyInjection;

namespace Feature.ElectionRounds.UnitTests.ValidatorTests;

public class ListRequestValidatorTests
{
    private readonly List.Validator _validator;
    private readonly DateTime _now = new(2024, 01, 02, 03, 04, 05, DateTimeKind.Utc);

    public ListRequestValidatorTests()
    {
        _validator = Factory.CreateValidator<List.Validator>(x =>
        {
            x.AddScoped<ITimeProvider>(_ => new FreezeTimeProvider(_now));
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validation_ShouldPass_When_PageSize_ValidValues(int pageSize)
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = pageSize,
            PageNumber = 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-5)]
    public void Validation_ShouldFail_When_PageSize_InvalidValues(int pageSize)
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = pageSize,
            PageNumber = 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void Validation_ShouldPass_When_PageNumber_ValidValues(int pageNumber)
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = 10,
            PageNumber = pageNumber
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validation_ShouldFail_When_PageNumber_InvalidValues(int pageNumber)
    {
        // Arrange
        var request = new List.Request
        {
            PageSize = 10,
            PageNumber = pageNumber
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    [Fact]
    public void Validation_ShouldPass_When_EmptyCountryId()
    {
        // Arrange
        var request = new List.Request { CountryId = null };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CountryId);
    }
    [Fact]
    public void Validation_ShouldFail_When_UnknownCountryId()
    {
        // Arrange
        var request = new List.Request { CountryId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidCountryId()
    {
        // Arrange
        var request = new List.Request { CountryId = CountriesList.MD.Id };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CountryId);
    }

}
