using System.Security.Claims;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class InitiateUploadEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRepository<QuickReportAttachment> _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IReadRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly InitiateUpload.Endpoint _endpoint;

    public InitiateUploadEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IRepository<QuickReportAttachment>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _monitoringObserverRepository = Substitute.For<IReadRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<InitiateUpload.Endpoint>(_authorizationService,
            _repository,
            _fileStorageService,
            _monitoringObserverRepository,
            new CurrentUtcTimeProvider());
    }

    [Fact]
    public async Task ShouldInitiateFileUpload_AndReturnPresignedUrl()
    {
        // Arrange
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverIdSpecification>())
            .Returns(Guid.NewGuid());

        var request = new InitiateUpload.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            ContentType = "text/plain",
            FileName = "test.txt",
            LastUpdatedAt = DateTime.UtcNow,
            NumberOfUploadParts = 3
        };

        var uploadPath = $"elections/{request.ElectionRoundId}/quick-reports/{request.QuickReportId}";


        var multipartUploadResult = new MultipartUploadResult()
        {
            UploadId = Guid.NewGuid().ToString(),
            PresignedUrls = new Dictionary<int, string>()
            {
                { 123, "http://localhost:5000" }, { 321, "http://localhost:5001" },
            }
        };

        _fileStorageService.CreateMultipartUploadAsync(uploadPath, Arg.Any<string>(), request.ContentType,
                request.NumberOfUploadParts)
            .Returns(multipartUploadResult);

        // Act

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<QuickReportAttachment>(x => x.Id == request.Id));

        var model = result.Result.As<Ok<InitiateUpload.Response>>();
        model.Value!.UploadId.Should().Be(multipartUploadResult.UploadId);
        model.Value!.UploadUrls.Should().BeEquivalentTo(multipartUploadResult.PresignedUrls);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new InitiateUpload.Request();
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<InitiateUpload.Response>, NotFound, Conflict>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
