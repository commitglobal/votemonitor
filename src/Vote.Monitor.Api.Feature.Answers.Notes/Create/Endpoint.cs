namespace Vote.Monitor.Api.Feature.Answers.Notes.Create;

public class Endpoint(IRepository<AnswerNoteAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<NoteModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("answer-notes"));
    }

    public override async Task<Ok<NoteModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
