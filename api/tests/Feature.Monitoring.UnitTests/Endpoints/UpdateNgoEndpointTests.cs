using Feature.Monitoring.Specifications;
using Feature.Monitoring.Update;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.Monitoring.UnitTests.Endpoints;

public class UpdateNgoEndpointTests
{
    private readonly IRepository<MonitoringNgoAggregate> _repository =
        Substitute.For<IRepository<MonitoringNgoAggregate>>();

    private readonly Endpoint _endpoint;

    public UpdateNgoEndpointTests()
    {
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMonitoringNgoNotFound()
    {
        // Arrange
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid(),
            AllowMultipleFormSubmission = true
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldUpdateAllowMultipleFormSubmission()
    {
        // Arrange
        var electionRoundId = Guid.NewGuid();
        var ngo = new NgoAggregateFaker(status: NgoStatus.Activated).Generate();
        var monitoringNgo = new MonitoringNgoAggregateFaker(electionRound: new ElectionRoundAggregateFaker(id: electionRoundId).Generate(), ngo: ngo).Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(monitoringNgo);

        var request = new Request
        {
            ElectionRoundId = electionRoundId,
            NgoId = ngo.Id,
            AllowMultipleFormSubmission = true
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();

        monitoringNgo.AllowMultipleFormSubmission.Should().BeTrue();
        await _repository.Received(1).UpdateAsync(monitoringNgo);
    }
}
