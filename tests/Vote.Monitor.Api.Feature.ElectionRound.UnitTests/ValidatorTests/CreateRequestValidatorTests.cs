﻿using Microsoft.Extensions.DependencyInjection;
using Vote.Monitor.TestUtils.Utils;

namespace Vote.Monitor.Api.Feature.ElectionRound.UnitTests.ValidatorTests;

public class CreateRequestValidatorTests
{
    private readonly Create.Validator _validator;
    private readonly DateTime _now = new(2024, 01, 02, 03, 04, 05, DateTimeKind.Utc);
    public CreateRequestValidatorTests()
    {
        _validator = Factory.CreateValidator<Create.Validator>(x =>
         {
             x.AddScoped<ITimeProvider>(_ => new FreezeTimeProvider(_now));
         });
    }

    [Fact]
    public void Validation_ShouldPass_When_Title_NotEmpty()
    {
        // Arrange
        var request = new Create.Request { Title = Guid.NewGuid().ToString() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_Title_Empty(string title)
    {
        // Arrange
        var request = new Create.Request { Title = title };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldFail_When_Title_Exceeds_Limit()
    {
        // Arrange
        var request = new Create.Request { Title = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
    [Fact]
    public void Validation_ShouldFail_When_Title_Below_Limit()
    {
        // Arrange
        var request = new Create.Request { Title = "aa" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Validation_ShouldPass_When_EnglishTitle_NotEmpty()
    {
        // Arrange
        var request = new Create.Request { EnglishTitle = Guid.NewGuid().ToString() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.EnglishTitle);
    }

    [Theory]
    [MemberData(nameof(TestData.EmptyStringsTestCases), MemberType = typeof(TestData))]
    public void Validation_ShouldFail_When_EnglishTitle_Empty(string title)
    {
        // Arrange
        var request = new Create.Request { EnglishTitle = title };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EnglishTitle);
    }

    [Fact]
    public void Validation_ShouldFail_When_EnglishTitle_Exceeds_Limit()
    {
        // Arrange
        var request = new Create.Request { EnglishTitle = "a".Repeat(257) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EnglishTitle);
    }

    [Fact]
    public void Validation_ShouldFail_When_EnglishTitle_Below_Limit()
    {
        // Arrange
        var request = new Create.Request { EnglishTitle = "aa" };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EnglishTitle);
    }

    [Theory]
    [MemberData(nameof(InvalidStartDates))]
    public void Validation_ShouldFail_When_StartDateInThePast(DateOnly startDate)
    {
        // Arrange
        var request = new Create.Request { StartDate = startDate };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validation_ShouldPass_When_StartDateInTheFuture()
    {
        // Arrange
        var request = new Create.Request { StartDate = new(2024, 01, 11) };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validation_ShouldFail_When_UnknownCountryId()
    {
        // Arrange
        var request = new Create.Request { CountryId = Guid.NewGuid() };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CountryId);
    }

    [Fact]
    public void Validation_ShouldPass_When_ValidCountryId()
    {
        // Arrange
        var request = new Create.Request { CountryId = CountriesList.MD.Id };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CountryId);
    }

    public static IEnumerable<object[]> InvalidStartDates =>
        new List<object[]>
        {
            new object[] { new DateOnly(2024,01,01)},
            new object[] { new DateOnly(2024,01,02)}
        };
}
