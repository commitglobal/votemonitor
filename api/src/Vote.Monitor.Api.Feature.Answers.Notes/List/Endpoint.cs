namespace Vote.Monitor.Api.Feature.Answers.Notes.List;

public class Endpoint(IReadRepository<AnswerNoteAggregate> repository) : Endpoint<Request, Results<Ok<Response>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/notes");
        DontAutoTag();
        Options(x => x.WithTags("answer-notes"));
    }

    public override async Task<Results<Ok<Response>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
