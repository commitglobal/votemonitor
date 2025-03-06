using System.Security.Claims;
using FastEndpoints;
using Feature.Attachments.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Attachments.UnitTests.Endpoints;

public class CompleteUploadEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<AttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly CompleteUpload.Endpoint _endpoint;

    public CompleteUploadEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<AttachmentAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<CompleteUpload.Endpoint>(_authorizationService,
            _repository,
            _fileStorageService);
    }

    [Fact]
    public async Task Should_Complete_Upload_AndReturnNoContent()
    {
        // Arrange
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        var attachment = new AttachmentFaker().Generate();
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetAttachmentByIdSpecification>())
            .Returns(attachment);

        // Act

        var request = new CompleteUpload.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            UploadId = Guid.NewGuid().ToString(),
            Etags = new Dictionary<int, string>() { { 123, "some value" }, { 321, "another value" }, }
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<AttachmentAggregate>(x => x.IsCompleted));

        await _fileStorageService.CompleteUploadAsync(request.UploadId, attachment.FilePath,
            attachment.UploadedFileName, request.Etags,
            CancellationToken.None);

        result
            .As<Results<NoContent, NotFound>>()
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new CompleteUpload.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Id = Guid.NewGuid(),
            UploadId = Guid.NewGuid().ToString(),
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should()
            .BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAttachmentNotFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        // Act
        var request = new CompleteUpload.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
            Id = Guid.NewGuid(),
            UploadId = Guid.NewGuid().ToString(),
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should()
            .BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
