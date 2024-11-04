using Feature.IncidentReports.Requests;

namespace Feature.IncidentReports.ListFormsOverview;

public class Endpoint(IAuthorizationService authorizationService, VoteMonitorContext context)
    : Endpoint<IncidentReportsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports:byForm");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminsOnly);

        Summary(x => { x.Summary = "Incident reports aggregated by form"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(IncidentReportsAggregateFilter req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var aggregatedFormOverviews = await context
            .IncidentReports
            .Where(x => x.PollingStation != null && (string.IsNullOrWhiteSpace(req.Level1Filter) ||
                                                     EF.Functions.ILike(x.PollingStation.Level1, req.Level1Filter)))
            .Where(x => x.PollingStation != null && (string.IsNullOrWhiteSpace(req.Level2Filter) ||
                                                     EF.Functions.ILike(x.PollingStation.Level2, req.Level2Filter)))
            .Where(x => x.PollingStation != null && (string.IsNullOrWhiteSpace(req.Level3Filter) ||
                                                     EF.Functions.ILike(x.PollingStation.Level3, req.Level3Filter)))
            .Where(x => x.PollingStation != null && (string.IsNullOrWhiteSpace(req.Level4Filter) ||
                                                     EF.Functions.ILike(x.PollingStation.Level4, req.Level4Filter)))
            .Where(x => x.PollingStation != null && (string.IsNullOrWhiteSpace(req.Level5Filter) ||
                                                     EF.Functions.ILike(x.PollingStation.Level5, req.Level5Filter)))
            .Where(x => req.HasFlaggedAnswers == null || (req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0))
            .Where(x => req.QuestionsAnswered == null
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.All &&
                            x.NumberOfQuestionsAnswered == x.Form.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.Some &&
                            x.NumberOfQuestionsAnswered < x.Form.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.None && x.NumberOfQuestionsAnswered == 0))
            .Where(x => req.HasNotes == null || (req.HasNotes.Value
                ? x.Notes.Any()
                : !x.Notes.Any()))
            .Where(x => req.HasAttachments == null || (req.HasAttachments.Value
                ? x.Attachments.Any()
                : !x.Attachments.Any()))
            .Where(x => req.FollowUpStatusFilter == null || x.FollowUpStatus == req.FollowUpStatusFilter)
            .Where(x => req.LocationTypeFilter == null || x.LocationType == req.LocationTypeFilter)
            .Where(x => req.IsCompletedFilter == null || x.IsCompleted == req.IsCompletedFilter)
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

        return TypedResults.Ok(new Response { AggregatedForms = aggregatedFormOverviews });
    }
}