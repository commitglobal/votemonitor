namespace Vote.Monitor.Api.Feature.Answers.Attachments.Create;

public class Endpoint(IRepository<AnswerAttachmentAggregate> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Ok<AttachmentModel>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/forms/{formId}/questions/{questionId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments"));
        AllowFileUploads();
    }

    public override async Task<Ok<AttachmentModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
