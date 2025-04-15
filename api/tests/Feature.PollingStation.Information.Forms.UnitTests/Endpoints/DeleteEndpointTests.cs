using FastEndpoints;
using Feature.PollingStation.Information.Forms.Specifications;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using FluentAssertions;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Xunit;

namespace Feature.PollingStation.Information.Forms.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<PollingStationInformationForm> _repository;
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationInformationForm>>();
        _endpoint = Factory.Create<Delete.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_DeletePollingStationInformationForm_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var form = new PollingStationInformationFormFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .Returns(form);

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository.Received(1).DeleteAsync(form);

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationFormNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationFormSpecification>())
            .ReturnsNull();

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<PollingStationInformationForm>());

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
