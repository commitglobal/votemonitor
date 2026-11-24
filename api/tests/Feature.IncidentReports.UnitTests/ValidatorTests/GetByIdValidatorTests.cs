using FastEndpoints;
using Feature.IncidentReports.GetById;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Vote.Monitor.Core.Services.Security;
using Xunit;

namespace Feature.IncidentReports.UnitTests.ValidatorTests;

public class GetByIdValidatorTests
{
    private readonly ICurrentUserRoleProvider _roleProvider = Substitute.For<ICurrentUserRoleProvider>();
    private readonly Validator _validator;

    public GetByIdValidatorTests()
    {
        _validator = Factory.CreateValidator<Validator>(x => x.AddScoped(_ => _roleProvider));
    }

    [Fact]
    public void Should_Have_Error_When_ElectionRoundId_Is_Empty()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ElectionRoundId);
    }

    [Theory]
    [InlineData(true, false, null)]
    [InlineData(true, false, "00000000-0000-0000-0000-000000000000")]
    [InlineData(false, true, null)]
    [InlineData(false, true, "00000000-0000-0000-0000-000000000000")]
    public void Should_Have_Error_When_NgoId_Is_Missing_For_NgoAdmin_Or_NgoStaff(bool isNgoAdmin, bool isNgoStaff,
        string? ngoId)
    {
        // Arrange
        _roleProvider.IsNgoAdmin().Returns(isNgoAdmin);
        _roleProvider.IsNgoStaff().Returns(isNgoStaff);
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = ngoId is null ? null : Guid.Parse(ngoId),
            IncidentReportId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NgoId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_NgoId_Is_Missing_For_PlatformAdmin()
    {
        // Arrange: platform admins have no NGO
        _roleProvider.IsPlatformAdmin().Returns(true);
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = null,
            IncidentReportId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_IncidentReportId_Is_Empty()
    {
        // Arrange
        var request = new Request
        {
            IncidentReportId = Guid.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.IncidentReportId);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Should_Not_Have_Error_When_All_Fields_Are_Valid_For_NgoAdmin_Or_NgoStaff(bool isNgoAdmin,
        bool isNgoStaff)
    {
        // Arrange
        _roleProvider.IsNgoAdmin().Returns(isNgoAdmin);
        _roleProvider.IsNgoStaff().Returns(isNgoStaff);
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            IncidentReportId = Guid.NewGuid()
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
