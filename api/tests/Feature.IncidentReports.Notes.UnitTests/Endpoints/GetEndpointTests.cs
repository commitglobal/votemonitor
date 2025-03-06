using Feature.IncidentReports.Notes.Get;
using Feature.IncidentReports.Notes.Specifications;
using Vote.Monitor.Domain.Entities.IncidentReportNoteAggregate;

namespace Feature.IncidentReports.Notes.UnitTests.Endpoints;

public class GetEndpointTests
{
    private readonly IReadRepository<IncidentReportNote> _repository;
    private readonly Endpoint _endpoint;

    public GetEndpointTests()
    {
        _repository = Substitute.For<IReadRepository<IncidentReportNote>>();
        _endpoint = Factory.Create<Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldReturnOkWithNoteModel_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new IncidentReportNoteFaker().Generate();

        _repository.FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var request = new Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            Id = fakeNote.Id
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        var model = result.Result.As<Ok<IncidentReportNoteModel>>();
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
        var request = new Request
        {
            ElectionRoundId = Guid.NewGuid(),
            Id = Guid.NewGuid()
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result
            .Should().BeOfType<Results<Ok<IncidentReportNoteModel>, NotFound>>()
            .Which
            .Result.Should().BeOfType<NotFound>();
    }
}