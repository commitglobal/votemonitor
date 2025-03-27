using FastEndpoints;
using Feature.CitizenReports.Notes.Specifications;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;
using Vote.Monitor.Domain.Repository;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Feature.CitizenReports.Notes.UnitTests.Endpoints;

public class UpsertEndpointTests
{
    private readonly IRepository<CitizenReportNote> _repository;
    private readonly Upsert.Endpoint _endpoint;

    public UpsertEndpointTests()
    {
        _repository = Substitute.For<IRepository<CitizenReportNote>>();
        _endpoint = Factory.Create<Upsert.Endpoint>(_repository);
    }

    [Fact]
    public async Task ShouldAddNote_WhenNoteNotFound()
    {
        // Arrange
        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var citizenReportId = Guid.NewGuid();
        var formId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        // Act
        var noteText = "a polling station note";
        var noteId = Guid.NewGuid();

        var request = new Upsert.Request
        {
            ElectionRoundId = fakeElectionRound.Id,
            CitizenReportId = citizenReportId,
            FormId = formId,
            QuestionId = questionId,
            Text = noteText,
            Id = noteId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .AddAsync(Arg.Is<CitizenReportNote>(x => x.Text == noteText
                                                     && x.ElectionRoundId == fakeElectionRound.Id
                                                     && x.CitizenReportId == citizenReportId
                                                     && x.FormId == formId
                                                     && x.QuestionId == questionId
                                                     && x.Id == noteId));

        var model = result.Result.As<Ok<CitizenReportNoteModel>>();
        model.Value!.Text.Should().Be(noteText);
        model.Value.UpdatedAt.Should().BeNull();
        model.Value.Id.Should().Be(noteId);
    }

    [Fact]
    public async Task ShouldUpdatedNote_WhenAllIdsExist()
    {
        // Arrange
        var fakeNote = new CitizenReportNoteFaker().Generate();

        _repository
            .FirstOrDefaultAsync(Arg.Any<GetNoteByIdSpecification>())
            .Returns(fakeNote);

        // Act
        var updatedText = "an updated note";
        var request = new Upsert.Request
        {
            ElectionRoundId = fakeNote.ElectionRoundId,
            CitizenReportId = fakeNote.CitizenReportId,
            FormId = fakeNote.FormId,
            Id = fakeNote.Id,
            Text = updatedText,
            QuestionId = fakeNote.QuestionId
        };
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _repository
            .Received(1)
            .UpdateAsync(Arg.Is<CitizenReportNote>(x => x.Text == updatedText
                                                     && x.ElectionRoundId == fakeNote.ElectionRoundId
                                                     && x.CitizenReportId == fakeNote.CitizenReportId
                                                     && x.FormId == fakeNote.FormId
                                                     && x.QuestionId == fakeNote.QuestionId));

        var model = result.Result.As<Ok<CitizenReportNoteModel>>();
        model.Value!.Text.Should().Be(updatedText);
        model.Value.Id.Should().Be(fakeNote.Id);
    }
}