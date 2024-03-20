using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.List;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<PollingStationAttachmentAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_repository,
            _fileStorageService,
            _electionRoundRepository,
            _pollingStationRepository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakePollingStationAttachment = new PollingStationAttachmentFaker(attachmentId, fileName).Generate();
        var anotherFakePollingStationAttachment = new PollingStationAttachmentFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        _repository
            .ListAsync(Arg.Any<GetPollingStationAttachmentsSpecification>(), CancellationToken.None)
            .Returns(new List<PollingStationAttachmentAggregate>()
            {
                fakePollingStationAttachment,
                anotherFakePollingStationAttachment
            });

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<List<AttachmentModel>>>();
        model.Value!.Count.Should().Be(2);
        model.Value.First().FileName.Should().Be(fakePollingStationAttachment.FileName);
        model.Value.First().Id.Should().Be(fakePollingStationAttachment.Id);
        model.Value.Last().FileName.Should().Be(anotherFakePollingStationAttachment.FileName);
        model.Value.Last().Id.Should().Be(anotherFakePollingStationAttachment.Id);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenElectionRoundDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns((ElectionRound)null!);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<AttachmentModel>>, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenPollingStationDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns((PollingStationAggregate)null!);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<AttachmentModel>>, BadRequest<ProblemDetails>>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenPollingStationAttachmentsDoNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakePollingStation = new PollingStationFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _electionRoundRepository
            .GetByIdAsync(fakeElectionRound.Id)
            .Returns(fakeElectionRound);

        _pollingStationRepository
            .FirstOrDefaultAsync(Arg.Any<GetPollingStationSpecification>())
            .Returns(fakePollingStation);

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        _repository
            .ListAsync(Arg.Any<GetPollingStationAttachmentsSpecification>(), CancellationToken.None)
            .Returns(new List<PollingStationAttachmentAggregate>());

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<List<AttachmentModel>>>();
        model.Value!.Should().BeEmpty();
    }
}
