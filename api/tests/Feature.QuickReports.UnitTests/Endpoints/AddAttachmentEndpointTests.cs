using System.Net;
using System.Security.Claims;
using System.Text;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<QuickReportAttachment> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly AddAttachment.Endpoint _endpoint;
    private readonly Guid _attachmentId = Guid.NewGuid();
    private readonly Guid _quickReportId = Guid.NewGuid();
    public CreateEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<QuickReportAttachment>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<AddAttachment.Endpoint>(_authorizationService,
            _monitoringObserverRepository,
            _repository,
            _fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenUploadSucceeds()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

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

        var request = new AddAttachment.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            QuickReportId = _quickReportId,
            Id = _attachmentId,
            ObserverId = fakeMonitoringObserver.Id,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<QuickReportAttachment>(x => x.ElectionRoundId == fakeElectionRound.Id
                                                                      && x.MonitoringObserverId == fakeMonitoringObserver.Id
                                                                      && x.Id == _attachmentId
                                                                      && x.QuickReportId == _quickReportId));

        var model = result.Result.As<Ok<QuickReportAttachmentModel>>();
        model.Value!.PresignedUrl.Should().Be(url);
        model.Value.UrlValidityInSeconds.Should().Be(urlValidityInSeconds);
        model.Value.FileName.Should().Be(fileName);
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenUploadFails()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

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

        var request = new AddAttachment.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
            QuickReportId = _quickReportId,
            Id = _attachmentId,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .DidNotReceiveWithAnyArgs()
            .AddAsync(Arg.Any<QuickReportAttachment>());

        result
            .Should().BeOfType<Results<Ok<QuickReportAttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<StatusCodeHttpResult>();

        var model = result.Result.As<StatusCodeHttpResult>();
        model.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new AddAttachment.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            QuickReportId = _quickReportId,
            Id = _attachmentId,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<QuickReportAttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenMonitoringObserverDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .ReturnsNull();

        // Act
        var request = new AddAttachment.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            QuickReportId = _quickReportId,
            Id = _attachmentId,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<QuickReportAttachmentModel>, NotFound, BadRequest<ProblemDetails>, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
