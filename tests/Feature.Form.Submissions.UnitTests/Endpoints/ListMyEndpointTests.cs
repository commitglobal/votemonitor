using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class ListMyEndpointTests
{
    private readonly IReadRepository<FormSubmission> _repository;
    private readonly ListMy.Endpoint _endpoint;

    public ListMyEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<FormSubmission>>();
        _endpoint = Factory.Create<ListMy.Endpoint>(_repository);
    }

    [Fact]
    public async Task Should_GetPollingStationInformation_ForNgo()
    {
        // Arrange
        var formSubmissions = new FormSubmissionFaker()
            .Generate(10)
            .Select(FormSubmissionModel.FromEntity)
            .ToList();

        _repository
            .ListAsync(Arg.Any<GetFormSubmissionForObserverSpecification>())
            .Returns(formSubmissions);

        // Act
        var request = new ListMy.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            ObserverId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Ok<ListMy.Response>>()
            .Which.Value!.Submissions
            .Should().BeEquivalentTo(formSubmissions);
    }
}
