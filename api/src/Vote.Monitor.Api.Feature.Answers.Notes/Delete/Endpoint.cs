namespace Vote.Monitor.Api.Feature.Answers.Notes.Delete;

public class Endpoint(IRepository<AnswerNoteAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound, ProblemDetails>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/notes/{id}");
        DontAutoTag();
        Options(x => x.WithTags("answer-notes"));
    }

    public override async Task<Results<NoContent, NotFound, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
