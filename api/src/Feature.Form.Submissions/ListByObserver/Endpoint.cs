using Authorization.Policies;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.ListByObserver;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, PagedResponse<ObserverSubmissionsOverview>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions:byObserver");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x =>
        {
            x.Summary = "Form submissions aggregated by observer";
        });
    }

    public override async Task<PagedResponse<ObserverSubmissionsOverview>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
        //var specification = new ListFormSubmissionsSpecification(req);
        //var entries = await repository.ListAsync(specification, ct);
        //var entriesCount = await repository.CountAsync(specification, ct);

        //return new PagedResponse<ObserverSubmissionsOverview>(entries, entriesCount, req.PageNumber, req.PageSize);
    }
}
