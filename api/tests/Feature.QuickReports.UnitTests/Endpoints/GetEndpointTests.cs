using System.Security.Claims;
using Feature.QuickReports.Get;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<QuickReport> _repository;
    private readonly IReadRepository<QuickReportAttachment> _quickReportAttachmentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<QuickReport>>();
        _quickReportAttachmentRepository = Substitute.For<IReadRepository<QuickReportAttachment>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _endpoint = Factory.Create<Get.Endpoint>(_authorizationService,
            _repository,
            _quickReportAttachmentRepository,
            _fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithQuickReportDetailedModel_WhenAllIdsExist()
    {
        // Arrange
        var quickReportId = Guid.NewGuid();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var quickReport = new QuickReportFaker(quickReportId).Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.FirstOrDefaultAsync(Arg.Any<GetQuickReportByIdSpecification>())
            .Returns(quickReport);

        List<QuickReportAttachment> quickReportAttachments = [new QuickReportAttachmentFaker(), new QuickReportAttachmentFaker()];

        _quickReportAttachmentRepository
            .ListAsync(Arg.Any<ListQuickReportAttachmentsSpecification>())
            .Returns(quickReportAttachments);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = quickReportId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<QuickReportDetailedModel>>().Value!;

        model.Id.Should().Be(quickReport.Id);
        model.ElectionRoundId.Should().Be(quickReport.ElectionRoundId);
        model.QuickReportLocationType.Should().Be(quickReport.QuickReportLocationType);
        model.Title.Should().Be(quickReport.Title);
        model.Description.Should().Be(quickReport.Description);
        model.PollingStationId.Should().Be(quickReport.PollingStationId);
        model.PollingStationDetails.Should().Be(quickReport.PollingStationDetails);

        model.Attachments.Should().BeEquivalentTo(quickReportAttachments, cmp => cmp.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldGetPresignedUrlsForAttachments_WhenAllIdsExist()
    {
        // Arrange
        var quickReportId = Guid.NewGuid();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeQuickReport = new QuickReportFaker(quickReportId).Generate();

        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetQuickReportByIdSpecification>())
            .Returns(fakeQuickReport);

        var quickReportAttachment1 = new QuickReportAttachmentFaker().Generate();
        var quickReportAttachment2 = new QuickReportAttachmentFaker().Generate();
        List<QuickReportAttachment> quickReportAttachments = [quickReportAttachment1, quickReportAttachment2];

        _quickReportAttachmentRepository
            .ListAsync(Arg.Any<ListQuickReportAttachmentsSpecification>())
            .Returns(quickReportAttachments);
        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = quickReportId
        };
        _ = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _fileStorageService
            .Received(1)
            .GetPresignedUrlAsync(quickReportAttachment1.FilePath, quickReportAttachment1.UploadedFileName);

        await _fileStorageService
            .Received(1)
            .GetPresignedUrlAsync(quickReportAttachment2.FilePath, quickReportAttachment2.UploadedFileName);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var quickReportId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = quickReportId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<QuickReportDetailedModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAttachmentDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var quickReportId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository
            .GetByIdAsync(Arg.Any<Guid>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            Id = quickReportId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<QuickReportDetailedModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
