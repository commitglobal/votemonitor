using Feature.Form.Submissions.Services;
using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<FormSubmission> _repository;
    private readonly IOrphanedDataCleanerService _cleanerService;
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<FormSubmission>>();
        _cleanerService = Substitute.For<IOrphanedDataCleanerService>();
        _endpoint = Factory.Create<Delete.Endpoint>(_repository, _cleanerService);
    }

    [Fact]
    public async Task Should_DeleteFormSubmission_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var formSubmission = new FormSubmissionFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .Returns(formSubmission);

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = formSubmission.ElectionRoundId,
            ObserverId = Guid.NewGuid(),
            FormId = formSubmission.FormId,
            PollingStationId = formSubmission.PollingStationId
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.Received(1).DeleteAsync(formSubmission);

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NoContent>();
    }

    [Fact]
    public async Task Should_DeleteOrphanedData_WhenFormSubmissionExists()
    {
        // Arrange
        var formSubmission = new FormSubmissionFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .Returns(formSubmission);

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = formSubmission.ElectionRoundId,
            ObserverId = Guid.NewGuid(),
            FormId = formSubmission.FormId,
            PollingStationId = formSubmission.PollingStationId
        };
        _ = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _cleanerService
            .Received(1)
            .CleanupAsync(formSubmission.ElectionRoundId, formSubmission.MonitoringObserverId, formSubmission.PollingStationId, formSubmission.FormId);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenFormSubmissionNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionSpecification>())
            .ReturnsNull();

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        await _repository.DidNotReceiveWithAnyArgs().DeleteAsync(Arg.Any<FormSubmission>());

        result
            .Should().BeOfType<Results<NoContent, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
