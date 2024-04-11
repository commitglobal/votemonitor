using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<FormSubmission> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<FormSubmission>>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_GetPollingStationInformation_And_ReturnNoContent_WhenExists()
    {
        // Arrange
        var formSubmission = new FormSubmissionFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionById>())
            .Returns(formSubmission);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<Ok<FormSubmissionModel>>()
            .Which.Value.Should().BeEquivalentTo(formSubmission, opt => opt.ExcludingMissingMembers());
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenPollingStationInformationNotFound()
    {
        // Arrange
        _repository
            .FirstOrDefaultAsync(Arg.Any<GetFormSubmissionById>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert

        result
            .Should().BeOfType<Results<Ok<FormSubmissionModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}
