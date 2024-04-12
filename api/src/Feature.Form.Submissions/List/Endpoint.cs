namespace Feature.Form.Submissions.List;

public class Endpoint(IReadRepository<FormSubmission> repository) : Endpoint<Request, Ok<Response>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
    }

    public override async Task<Ok<Response>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetFormSubmissions(req);
        var infos = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(new Response
        {
            Submissions = infos
        });
    }
}
