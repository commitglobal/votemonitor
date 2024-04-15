namespace Vote.Monitor.Api.Feature.Answers.Attachments.Delete;

public class Endpoint(IRepository<AnswerAttachmentAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("answers-attachments"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
