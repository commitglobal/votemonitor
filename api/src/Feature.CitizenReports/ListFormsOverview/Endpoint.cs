using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.CitizenReports.ListFormsOverview;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports:byForm");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x => { x.Summary = "Form submissions aggregated by observer"; });
    }

    public override async Task<Response> ExecuteAsync(Request req, CancellationToken ct)
    {
        var aggregatedFormOverviews = await context
            .CitizenReports
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.ElectionRound.CitizenReportingEnabled
                        && x.ElectionRound.MonitoringNgoForCitizenReporting.NgoId == req.NgoId)
            .GroupBy(cr => new { cr.FormId, cr.Form.Code, cr.Form.Name, cr.Form.DefaultLanguage })
            .Select(cr => new AggregatedFormOverview
            {
                FormId = cr.Key.FormId,
                FormCode = cr.Key.Code,
                FormName = cr.Key.Name,
                FormDefaultLanguage = cr.Key.DefaultLanguage,
                NumberOfNotes = cr.Sum(x => x.Notes.Count),
                NumberOfMediaFiles = cr.Sum(x => x.Attachments.Count),
                NumberOfSubmissions = cr.Count(),
                NumberOfFlaggedAnswers = cr.Sum(x => x.Answers.Count)
            })
            .ToListAsync(ct);

        return new Response { AggregatedForms = aggregatedFormOverviews };
    }
}