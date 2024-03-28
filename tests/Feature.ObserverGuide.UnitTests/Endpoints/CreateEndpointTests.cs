using System.Net;
using System.Security.Claims;
using System.Text;
using FastEndpoints;
using Feature.ObserverGuide.Create;
using Feature.ObserverGuide.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.ObserverGuide.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<ObserverGuideAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository;
    private readonly Create.Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<ObserverGuideAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _currentUserProvider = Substitute.For<ICurrentUserProvider>();
        _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();

        _endpoint = Factory.Create<Create.Endpoint>(_authorizationService,
            _repository,
            _currentUserProvider,
            _monitoringNgoRepository,
            _fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithObserverGuideModel_WhenUploadSucceeds()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

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

        var observerGuideTitle = "my observer guide";

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = observerGuideTitle,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<ObserverGuideAggregate>(x => x.Title == observerGuideTitle
                                                                      && x.MonitoringNgoId == fakeMonitoringNgo.Id));

        var model = result.Result.As<Ok<ObserverGuideModel>>();
        model.Value!.PresignedUrl.Should().Be(url);
        model.Value.UrlValidityInSeconds.Should().Be(urlValidityInSeconds);
        model.Value.FileName.Should().Be(fileName);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

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

        var observerGuideTitle = "my observer guide";

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = observerGuideTitle,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(0)
            .AddAsync(Arg.Is<ObserverGuideAggregate>(x => x.Title == observerGuideTitle
                                                          && x.MonitoringNgoId == fakeMonitoringNgo.Id));

        result
            .Should().BeOfType<Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMonitoringNgoNotFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .ReturnsNull();

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

        var observerGuideTitle = "my observer guide";

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = observerGuideTitle,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(0)
            .AddAsync(Arg.Is<ObserverGuideAggregate>(x => x.Title == observerGuideTitle
                                                          && x.MonitoringNgoId == fakeMonitoringNgo.Id));

        result
            .Should().BeOfType<Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenUploadFails()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);


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

        var observerGuideTitle = "my observer guide";

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = observerGuideTitle,
            Attachment = formFile
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository
            .Received(0)
            .AddAsync(Arg.Is<ObserverGuideAggregate>(x => x.Title == observerGuideTitle
                                                          && x.MonitoringNgoId == fakeMonitoringNgo.Id));

        result
            .Should().BeOfType<Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<StatusCodeHttpResult>();

        var model = result.Result.As<StatusCodeHttpResult>();
        model.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }
}
