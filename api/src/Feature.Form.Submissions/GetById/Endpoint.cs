using Authorization.Policies;

namespace Feature.Form.Submissions.GetById;

public class Endpoint(IReadRepository<FormSubmission> repository) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets submission by id";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
