namespace Vote.Monitor.Api.Feature.Answers.Attachments.List;

public class Endpoint(IReadRepository<AnswerAttachmentAggregate> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("answers-attachments"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
