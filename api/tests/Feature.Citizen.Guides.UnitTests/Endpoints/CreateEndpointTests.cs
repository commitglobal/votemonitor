using System.Net;
using System.Security.Claims;
using FastEndpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Citizen.Guides.UnitTests.Endpoints;

public class CreateEndpointTests
{
    private readonly IRepository<CitizenGuide> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHtmlStringSanitizer _htmlStringSanitizer;
    private readonly Create.Endpoint _endpoint;

    public CreateEndpointTests()
    {
        _repository = Substitute.For<IRepository<CitizenGuide>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _authorizationService = Substitute.For<IAuthorizationService>();
        _htmlStringSanitizer = Substitute.For<IHtmlStringSanitizer>();

        _endpoint = Factory.Create<Create.Endpoint>(_authorizationService,
            _repository,
            _fileStorageService,
            _htmlStringSanitizer);
    }

    [Fact]
    public async Task ShouldReturnOkWithCitizenGuideModel_WhenUploadSucceeds()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        var fileName = "file.txt";
        var url = "url";
        var urlValidityInSeconds = 60;

        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(),
                Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Ok(url, fileName, urlValidityInSeconds));

        // Act
        var guideTitle = "my citizen guide";

        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = guideTitle,
            Attachment = FakeFormFile.New().HavingFileName(fileName).Please(),
            GuideType = CitizenGuideType.Document
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<CitizenGuide>(x => x.Title == guideTitle));

        var model = result.Result.As<Ok<CitizenGuideModel>>();
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
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = "my citizen guide",
            Attachment = FakeFormFile.New().Please()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .DidNotReceive()
            .AddAsync(Arg.Any<CitizenGuide>());

        result
            .Should().BeOfType<Results<Ok<CitizenGuideModel>, NotFound, StatusCodeHttpResult>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnInternalServerError_WhenUploadFails()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _fileStorageService.UploadFileAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<Stream>(),
                Arg.Any<CancellationToken>())
            .Returns(new UploadFileResult.Failed("error message"));

        // Act
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = "my citizen guide",
            Attachment = FakeFormFile.New().Please(),
            GuideType = CitizenGuideType.Document
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .DidNotReceive()
            .AddAsync(Arg.Any<CitizenGuide>());

        result
            .Should().BeOfType<Results<Ok<CitizenGuideModel>, NotFound, StatusCodeHttpResult>>()
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

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        // Act
        var guideText = "<p>some html</p>";
        var request = new Create.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Title = "my citizen guide",
            Text = guideText,
            GuideType = CitizenGuideType.Text
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        _htmlStringSanitizer
            .Received(1)
            .Sanitize(guideText);
    }
}