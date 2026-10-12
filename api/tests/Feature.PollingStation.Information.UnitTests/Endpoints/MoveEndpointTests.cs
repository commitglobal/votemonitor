using Feature.PollingStation.Information.Move;
using Feature.PollingStation.Information.Specifications;

namespace Feature.PollingStation.Information.UnitTests.Endpoints;

public class MoveEndpointTests
{
    private readonly IRepository<PollingStationInformation> _repository;
    private readonly IReadRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly Endpoint _endpoint;

    public MoveEndpointTests()
    {
        _repository = Substitute.For<IRepository<PollingStationInformation>>();
        _pollingStationRepository = Substitute.For<IReadRepository<PollingStationAggregate>>();
        _endpoint = Factory.Create<Endpoint>(_repository, _pollingStationRepository);
    }

    [Fact]
    public async Task ShouldMovePollingStationInformation_WhenTargetHasNoPsiForObserver()
    {
        // Arrange
        var psi = new PollingStationInformationFaker().Generate();
        var target = new PollingStationFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdForUpdateSpecification>())
            .Returns(psi);
        _pollingStationRepository.FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>()).Returns(target);
        _repository.AnyAsync(Arg.Any<HasPollingStationInformationAtPollingStationSpecification>()).Returns(false);

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = psi.Id, PollingStationId = target.Id };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NoContent>();
        psi.PollingStationId.Should().Be(target.Id);
        await _repository.Received(1).UpdateAsync(psi);
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenObserverAlreadyHasPsiAtTarget()
    {
        // Arrange
        var psi = new PollingStationInformationFaker().Generate();
        var originalPollingStationId = psi.PollingStationId;
        var target = new PollingStationFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdForUpdateSpecification>())
            .Returns(psi);
        _pollingStationRepository.FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>()).Returns(target);
        _repository.AnyAsync(Arg.Any<HasPollingStationInformationAtPollingStationSpecification>()).Returns(true);

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = psi.Id, PollingStationId = target.Id };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<Conflict<string>>();
        psi.PollingStationId.Should().Be(originalPollingStationId);
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdForUpdateSpecification>())
            .ReturnsNull();

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = Guid.NewGuid(), PollingStationId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenTargetPollingStationNotFound()
    {
        // Arrange
        var psi = new PollingStationInformationFaker().Generate();
        _repository.FirstOrDefaultAsync(Arg.Any<GetPollingStationInformationByIdForUpdateSpecification>())
            .Returns(psi);
        _pollingStationRepository.FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>()).ReturnsNull();

        // Act
        var request = new Request { ElectionRoundId = Guid.NewGuid(), Id = psi.Id, PollingStationId = Guid.NewGuid() };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Result.Should().BeOfType<NotFound>();
        await _repository.DidNotReceiveWithAnyArgs().UpdateAsync(default!);
    }
}
