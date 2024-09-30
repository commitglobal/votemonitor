namespace Feature.IssueReports.Notes.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<IssueReportNote> _repository;
    private readonly Get.Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IssueReportNote>>();
        _endpoint = Factory.Create<Get.Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new IssueReportNoteFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            Id = fakeNote.Id
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        var model = result.Result.As<Ok<IssueReportNoteModel>>();
        model.Value!.Text.Should().Be(fakeNote.Text);
        model.Value.Id.Should().Be(fakeNote.Id);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenNotExists()
    {
        // Arrange
        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .ReturnsNull();

        // Act
        var request = new Get.Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, default);

        // Assert
        result
            .Should().BeOfType<Results<Ok<IssueReportNoteModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}