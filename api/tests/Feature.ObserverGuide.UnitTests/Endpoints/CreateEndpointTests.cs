using System.Net;
using System.Security.Claims;
using FastEndpoints;
using Feature.ObserverGuide.Create;
using Feature.ObserverGuide.Model;
using Feature.ObserverGuide.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.ObserverGuide.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<ObserverGuideAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<MonitoringNgo> _monitoringNgoRepository;
    private readonly Create.Endpoint _endpoint;
    private readonly IHtmlStringSanitizer _htmlStringSanitizer;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<ObserverGuideAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _monitoringNgoRepository = Substitute.For<IReadRepository<MonitoringNgo>>();
        _htmlStringSanitizer = Substitute.For<IHtmlStringSanitizer>();

        _endpoint = Factory.Create<Create.Endpoint>(_authorizationService,
            _repository,
            _monitoringNgoRepository,
            _fileStorageService,
            _htmlStringSanitizer);
    }

    [Fact]
    public async Task ShouldReturnOkWithObserverGuideModel_WhenUploadSucceeds()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

        var fileName = "file.txt";
        var url = "url";
        var urlValidityInSeconds = 60;
        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(),
                Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Ok(url, fileName, urlValidityInSeconds));

        // Act
        var observerGuideTitle = "my observer guide";

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = observerGuideTitle,
            Attachment = FakeFormFile.New().HavingFileName(fileName).Please(),
            GuideType = ObserverGuideType.Document
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

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

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = "my observer guide",
            Attachment = FakeFormFile.New().Please()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .DidNotReceive()
            .AddAsync(Arg.Any<ObserverGuideAggregate>());

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

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .ReturnsNull();

        var fileName = "file.txt";
        var url = "url";
        var urlValidityInSeconds = 60;
        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(),
                Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Ok(url, fileName, urlValidityInSeconds));

        // Act
        var observerGuideTitle = "my observer guide";

        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = observerGuideTitle,
            Attachment = FakeFormFile.New().Please()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .DidNotReceive()
            .AddAsync(Arg.Any<ObserverGuideAggregate>());

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

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(),
                Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Failed("error message"));

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = "my observer guide",
            Attachment = FakeFormFile.New().Please(),
            GuideType = ObserverGuideType.Document
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .DidNotReceive()
            .AddAsync(Arg.Any<ObserverGuideAggregate>());

        result
            .Should().BeOfType<Results<Ok<ObserverGuideModel>, NotFound, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<StatusCodeHttpResult>();

        var model = result.Result.As<StatusCodeHttpResult>();
        model.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task ShouldSanitizeHtml_WhenAttachmentTypeText()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringNgo = new MonitoringNgoAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringNgoRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringNgoSpecification>())
            .Returns(fakeMonitoringNgo);

        // Act
        var guideText = "<p>some html</p>";
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = "my citizen guide",
            Text = guideText,
            GuideType = ObserverGuideType.Text
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        _htmlStringSanitizer
            .Received(1)
            .Sanitize(guideText);
    }
}