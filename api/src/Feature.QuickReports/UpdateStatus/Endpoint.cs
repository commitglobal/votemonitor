using Authorization.Policies;
using Feature.QuickReports.Specifications;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.UpdateStatus;

public class Endpoint(IRepository<QuickReport> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Put("/api/election-rounds/{electionRoundId}/quick-reports/{id}:status");
        DontAutoTag();
        Options(x => x.WithTags("quick-reports"));
        Summary(s =>
        {
            s.Summary = "Updates follow up status for a quick report";
        });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetQuickReportByIdSpecification(req.ElectionRoundId, req.NgoId, req.Id);
        var quickReport = await repository.FirstOrDefaultAsync(specification, ct);

        if (quickReport is null)
        {
            return TypedResults.NotFound();
        }

        quickReport.UpdateFollowUpStatus(req.FollowUpStatus);

        await repository.UpdateAsync(quickReport, ct);
        return TypedResults.NoContent();
    }
}
