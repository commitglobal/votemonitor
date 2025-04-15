using FastEndpoints;
using Feature.PollingStation.Information.Forms.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Xunit;

namespace Feature.PollingStation.Information.Forms.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<PollingStationInformationForm> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationInformationForm>>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_GetPollingStationInformationForm_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var pollingStationInformation = new PollingStationInformationFormFaker().Generate();
        _repository
            .FirstOrDefaultAsync(Arg.Any<PollingStationInformationModelSpecification>())
            .Returns(PollingStationInformationFormModel.FromEntity(pollingStationInformation));

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<PollingStationInformationFormModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<PollingStationInformationFormModel>>()
            .Which.Value.Should().BeEquivalentTo(pollingStationInformation, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationFormNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<PollingStationInformationModelSpecification>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert

        result
            .Should().BeOfType<Results<Ok<PollingStationInformationFormModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
