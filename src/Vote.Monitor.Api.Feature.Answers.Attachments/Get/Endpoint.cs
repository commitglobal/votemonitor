namespace Vote.Monitor.Api.Feature.Answers.Attachments.Get;

public class Endpoint(IReadRepository<AnswerAttachmentAggregate> repository) : Endpoint<Request, Results<Ok<AttachmentModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("answers-attachments"));
    }

    public override async Task<Results<Ok<AttachmentModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
