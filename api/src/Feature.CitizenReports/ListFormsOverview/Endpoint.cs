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
            .Where(x => string.IsNullOrWhiteSpace(req.Level1Filter) ||
                        EF.Functions.ILike(x.Location.Level1, req.Level1Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level2Filter) ||
                        EF.Functions.ILike(x.Location.Level2, req.Level2Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level3Filter) ||
                        EF.Functions.ILike(x.Location.Level3, req.Level3Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level4Filter) ||
                        EF.Functions.ILike(x.Location.Level4, req.Level4Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level5Filter) ||
                        EF.Functions.ILike(x.Location.Level5, req.Level5Filter))
            .Where(x => req.HasFlaggedAnswers == null || (req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0))
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
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
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
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