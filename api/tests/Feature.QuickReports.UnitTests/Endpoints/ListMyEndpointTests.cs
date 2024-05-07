using System.Security.Claims;
using Feature.QuickReports.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;
using Vote.Monitor.Domain.Entities.QuickReportAttachmentAggregate;

namespace Feature.QuickReports.UnitTests.Endpoints;

public class ListMyEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<QuickReport> _repository;
    private readonly IReadRepository<QuickReportAttachment> _quickReportAttachmentRepository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ListMy.Endpoint _endpoint;

    public ListMyEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<QuickReport>>();
        _quickReportAttachmentRepository = Substitute.For<IReadRepository<QuickReportAttachment>>();
        _fileStorageService = Substitute.For<IFileStorageService>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();

        _endpoint = Factory.Create<ListMy.Endpoint>(_authorizationService,
            _repository,
            _quickReportAttachmentRepository,
            _fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithQuickReportModels_WhenAllIdsExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var quickReport1 = new QuickReportFaker().Generate();
        var quickReport2 = new QuickReportFaker().Generate();

        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository
            .ListAsync(Arg.Any<ListObserverQuickReportsSpecification>(), CancellationToken.None)
            .Returns([
                quickReport1,
                quickReport2
            ]);

        List<QuickReportAttachment> quickReport1Attachments = [
            new QuickReportAttachmentFaker(quickReportId: quickReport1.Id).Generate(),
            new QuickReportAttachmentFaker(quickReportId: quickReport1.Id).Generate(),
            new QuickReportAttachmentFaker(quickReportId: quickReport1.Id).Generate()
        ];

        List<QuickReportAttachment> quickReport2Attachments = [
            new QuickReportAttachmentFaker(quickReportId: quickReport2.Id).Generate(),
        ];
        List<QuickReportAttachment> attachments = [.. quickReport1Attachments, .. quickReport2Attachments];

        _quickReportAttachmentRepository
            .ListAsync(Arg.Any<ListQuickReportAttachmentsSpecification>())
            .Returns(attachments);

        // Act
        var request = new ListMy.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<List<QuickReportModel>>>().Value!;
        model.Should().HaveCount(2);

        model[0].Id.Should().Be(quickReport1.Id);
        model[0].ElectionRoundId.Should().Be(quickReport1.ElectionRoundId);
        model[0].QuickReportLocationType.Should().Be(quickReport1.QuickReportLocationType);
        model[0].Title.Should().Be(quickReport1.Title);
        model[0].Description.Should().Be(quickReport1.Description);
        model[0].PollingStationId.Should().Be(quickReport1.PollingStationId);
        model[0].PollingStationDetails.Should().Be(quickReport1.PollingStationDetails);

        model[0].Attachments.Should().BeEquivalentTo(quickReport1Attachments, cmp => cmp.ExcludingMissingMembers());

        model[1].Id.Should().Be(quickReport2.Id);
        model[1].ElectionRoundId.Should().Be(quickReport2.ElectionRoundId);
        model[1].QuickReportLocationType.Should().Be(quickReport2.QuickReportLocationType);
        model[1].Title.Should().Be(quickReport2.Title);
        model[1].Description.Should().Be(quickReport2.Description);
        model[1].PollingStationId.Should().Be(quickReport2.PollingStationId);
        model[1].PollingStationDetails.Should().Be(quickReport2.PollingStationDetails);

        model[1].Attachments.Should().BeEquivalentTo(quickReport2Attachments, cmp => cmp.ExcludingMissingMembers());
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
        var request = new ListMy.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<List<QuickReportModel>>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldGetPresignedUrlsForAttachments_WhenAllIdsExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakeQuickReport1 = new QuickReportFaker().Generate();
        var fakeQuickReport2 = new QuickReportFaker().Generate();

        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        List<QuickReport> quickReports = [fakeQuickReport1, fakeQuickReport2];

        _repository
            .ListAsync(Arg.Any<ListObserverQuickReportsSpecification>())
            .Returns(quickReports);

        var quickReportAttachment1 = new QuickReportAttachmentFaker().Generate();
        var quickReportAttachment2 = new QuickReportAttachmentFaker().Generate();
        List<QuickReportAttachment> quickReportAttachments = [quickReportAttachment1, quickReportAttachment2];

        _quickReportAttachmentRepository
            .ListAsync(Arg.Any<ListQuickReportAttachmentsSpecification>())
            .Returns(quickReportAttachments);

        // Act
        var request = new ListMy.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
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
    public async Task ShouldReturnEmptyList_WhenQuickReportsDoNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        _repository
            .ListAsync(Arg.Any<ListObserverQuickReportsSpecification>(), CancellationToken.None)
            .Returns([]);

        _quickReportAttachmentRepository
            .ListAsync(Arg.Any<ListQuickReportAttachmentsSpecification>(), CancellationToken.None)
            .Returns([]);

        // Act
        var request = new ListMy.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<List<QuickReportModel>>>();
        model.Value!.Should().BeEmpty();
    }
}
