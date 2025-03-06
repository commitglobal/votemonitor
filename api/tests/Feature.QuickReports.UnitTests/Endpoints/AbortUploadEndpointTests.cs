using System.Security.Claims;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class AbortUploadEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<QuickReportAttachment> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly AbortUpload.Endpoint _endpoint;

    public AbortUploadEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<QuickReportAttachment>>();
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

        var attachment = new QuickReportAttachmentFaker().Generate();
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetQuickReportAttachmentByIdSpecification>())
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
            .UpdateAsync(Arg.Is<QuickReportAttachment>(x => x.IsDeleted));

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
