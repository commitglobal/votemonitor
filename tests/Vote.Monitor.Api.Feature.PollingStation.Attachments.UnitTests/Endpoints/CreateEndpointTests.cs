using System.Net;
using System.Text;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Create;
using Vote.Monitor.Api.Feature.PollingStation.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;
using Endpoint = Vote.Monitor.Api.Feature.PollingStation.Attachments.Create.Endpoint;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly ITimeProvider _timeService;
    private readonly IRepository<PollingStationAttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepository<ElectionRound> _electionRoundRepository;
    private readonly IRepository<PollingStationAggregate> _pollingStationRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _timeService = Substitute.For<ITimeProvider>();
        _repository = Substitute.For<IRepository<PollingStationAttachmentAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _electionRoundRepository = Substitute.For<IRepository<ElectionRound>>();
        _pollingStationRepository = Substitute.For<IRepository<PollingStationAggregate>>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_repository,
            _fileStorageService,
            _electionRoundRepository,
            _pollingStationRepository,
            _monitoringObserverRepository,
            _timeService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenUploadSucceeds()
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

        var fileName = "file.txt";
        var bytes = Encoding.UTF8.GetBytes("Test content");
        var stream = new MemoryStream(bytes);
        var url = "url";
        var urlValidityInSeconds = 60;
        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Ok(url, fileName, urlValidityInSeconds));

        // Act
        var formFile = new FormFile(stream, 0, 0, string.Empty, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<PollingStationAttachmentAggregate>(x => x.ElectionRoundId == fakeElectionRound.Id
                                                                      && x.MonitoringObserverId == fakeMonitoringObserver.Id));

        var model = result.Result.As<Ok<AttachmentModel>>();
        model.Value!.PresignedUrl.Should().Be(url);
        model.Value.UrlValidityInSeconds.Should().Be(urlValidityInSeconds);
        model.Value.FileName.Should().Be(fileName);
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenUploadFails()
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

        var fileName = "file.txt";
        var bytes = Encoding.UTF8.GetBytes("Test content");
        var stream = new MemoryStream(bytes);
        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Failed("error message"));

        // Act
        var formFile = new FormFile(stream, 0, 0, string.Empty, fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpg"
        };

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = fakePollingStation.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(0)
            .AddAsync(Arg.Is<PollingStationAttachmentAggregate>(x => x.ElectionRoundId == fakeElectionRound.Id
                                                                     && x.MonitoringObserverId == fakeMonitoringObserver.Id));

        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<StatusCodeHttpResult>();

        var model = result.Result.As<StatusCodeHttpResult>();
        model.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
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
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
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
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenMonitoringObserverDoesNotExist()
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
            .Returns((MonitoringObserver)null!);

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
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
