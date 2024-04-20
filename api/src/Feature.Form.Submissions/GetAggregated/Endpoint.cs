using Authorization.Policies;

namespace Feature.Form.Submissions.GetAggregated;

public class Endpoint(IReadRepository<FormSubmission> repository) : Endpoint<Request, Results<Ok<FormAggregate>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{formId}:aggregated");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets aggregated form with all the notes and attachments";
        });
    }

    public override async Task<Results<Ok<FormAggregate>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
