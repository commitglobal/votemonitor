using Authorization.Policies;
using Vote.Monitor.Core.Models;

namespace Feature.Form.Submissions.ListEntries;

public class Endpoint(IReadRepository<FormSubmission> repository) : Endpoint<Request, PagedResponse<FormSubmissionEntries>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byEntry");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(x =>
        {
            x.Summary = "Lists form submissions by entry in our system";
        });
    }

    public override async Task<PagedResponse<FormSubmissionEntries>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListFormSubmissionEntriesSpecification(req);
        var entries = await repository.ListAsync(specification, ct);
        var entriesCount = await repository.CountAsync(specification, ct);

        return new PagedResponse<FormSubmissionEntries>(entries, entriesCount, req.PageNumber, req.PageSize);
    }
}
