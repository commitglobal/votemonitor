using Feature.IncidentReports.Comments.Specifications;
using Vote.Monitor.Domain.Entities.IncidentReportAggregate;

namespace Feature.IncidentReports.Comments.Create;

public class Endpoint(
    IReadRepository<IncidentReport> incidentReportRepository,
    IRepository<IncidentReportCommentAggregate> repository)
    : Endpoint<Request, Results<Ok<IncidentReportCommentModel>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/comments");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-comments"));
        Summary(s => { s.Summary = "Creates a comment for an incident report."; });

        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<IncidentReportCommentModel>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var incidentReportExists = await incidentReportRepository.AnyAsync(
            new GetIncidentReportForNgoSpecification(req.ElectionRoundId, req.NgoId, req.IncidentReportId), ct);

        if (!incidentReportExists)
        {
            return TypedResults.NotFound();
        }

        var comment = IncidentReportCommentAggregate.Create(
            req.ElectionRoundId,
            req.IncidentReportId,
            req.Text);

        await repository.AddAsync(comment, ct);

        return TypedResults.Ok(IncidentReportCommentModel.FromEntity(comment));
    }
}
