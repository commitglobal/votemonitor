namespace Vote.Monitor.Api.Feature.Answers.Notes.Update;

public class Endpoint(IReadRepository<AnswerNoteAggregate> repository) : Endpoint<Request, Results<Ok<NoteModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("answer-notes"));
    }

    public override async Task<Results<Ok<NoteModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
