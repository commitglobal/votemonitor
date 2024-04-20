using System.Security.Claims;
using FastEndpoints;
using Feature.Attachments.Get;
using Feature.Attachments.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain.Entities.MonitoringObserverAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.Attachments.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<AttachmentAggregate> _repository;
    private readonly IRepository<MonitoringObserver> _monitoringObserverRepository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _authorizationService = Substitute.For<IAuthorizationService>();
        _repository = Substitute.For<IReadRepository<AttachmentAggregate>>();
        var fileStorageService = Substitute.For<IFileStorageService>();
        _monitoringObserverRepository = Substitute.For<IRepository<MonitoringObserver>>();
        _endpoint = Factory.Create<Endpoint>(_authorizationService,
            _repository,
            fileStorageService);
    }

    [Fact]
    public async Task ShouldReturnOkWithAttachmentModel_WhenAllIdsExist()
    {
        // Arrange
        var attachmentId = Guid.NewGuid();
        var fileName = "photo.jpg";

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var fakePollingStationAttachment = new AttachmentFaker(attachmentId, fileName).Generate();
        var pollingStationId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _repository.GetByIdAsync(fakePollingStationAttachment.Id)
            .Returns(fakePollingStationAttachment);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = pollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
            Id = attachmentId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<AttachmentModel>>();
        model.Value!.FileName.Should().Be(fileName);
        model.Value.Id.Should().Be(attachmentId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotAuthorised()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var pollingStationId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Failed());

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = pollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenAttachmentDoesNotExist()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeMonitoringObserver = new MonitoringObserverFaker().Generate();
        var pollingStationId = Guid.NewGuid();

        _authorizationService.AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object?>(), Arg.Any<IEnumerable<IAuthorizationRequirement>>())
            .Returns(AuthorizationResult.Success());

        _monitoringObserverRepository
            .FirstOrDefaultAsync(Arg.Any<GetMonitoringObserverSpecification>())
            .Returns(fakeMonitoringObserver);

        _repository
            .GetByIdAsync(Arg.Any<Guid>())
            .ReturnsNull();

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            PollingStationId = pollingStationId,
            ObserverId = fakeMonitoringObserver.Id,
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<AttachmentModel>, BadRequest<ProblemDetails>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
