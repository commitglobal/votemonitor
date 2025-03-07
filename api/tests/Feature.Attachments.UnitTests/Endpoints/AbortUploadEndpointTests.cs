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

public class AbortUploadEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<AttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly AbortUpload.Endpoint _endpoint;

    public AbortUploadEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<AttachmentAggregate>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<AbortUpload.Endpoint>(_authorizationService,
            _repository,
            _fileStorageService);
    }

    [Fact]
    public async Task Should_Abort_Upload_AndReturnNoContent()
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

        var request = new AbortUpload.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            UploadId = Guid.NewGuid().ToString(),
        };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<AttachmentAggregate>(x => x.IsDeleted));

        await _fileStorageService.AbortUploadAsync(request.UploadId, attachment.FilePath, attachment.UploadedFileName,
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
        var request = new AbortUpload.Request
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
        var request = new AbortUpload.Request
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
