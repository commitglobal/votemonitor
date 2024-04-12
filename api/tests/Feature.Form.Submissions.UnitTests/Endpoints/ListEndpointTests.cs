using Vote.Monitor.Domain.Entities.FormSubmissionAggregate;

namespace Feature.Form.Submissions.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IReadRepository<FormSubmission> _repository;
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<FormSubmission>>();
        _endpoint = Factory.Create<List.Endpoint>(_repository);
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
            .ListAsync(Arg.Any<GetFormSubmissions>())
            .Returns(formSubmissions);

        // Act
        var request = new List.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            NgoId = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should()
            .BeOfType<Ok<List.Response>>()
            .Which.Value!.Submissions
            .Should().BeEquivalentTo(formSubmissions);
    }
}
