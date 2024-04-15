using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class DeleteEndpointTests
{
    private readonly IRepository<FormSubmission> _repository;
    private readonly Delete.Endpoint _endpoint;

    public DeleteEndpointTests()
    {
        _repository = Substitute.For<IRepository<FormSubmission>>();
        _endpoint = Factory.Create<Delete.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_DeletePollingStationInformation_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var formSubmission = new FormSubmissionFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionById>())
            .Returns(formSubmission);

        // Act
        var request = new Delete.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
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
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionById>())
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
