using Feature.IncidentReports.Requests;
using Vote.Monitor.Answer.Module.Aggregators;

namespace Feature.IncidentReports.GetSubmissionsAggregated;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IFileStorageService fileStorageService)
    : Endpoint<IncidentReportsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/forms/{formId}:aggregated-submissions");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets aggregated incident report form submissions with all the notes and attachments";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(IncidentReportsAggregateFilter req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new MonitoringNgoAdminRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var form = await context
            .Forms
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringNgo.NgoId == req.NgoId
                        && x.Id == req.FormId)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return await AggregateIncidentReportsAsync(form, req, ct);
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregateIncidentReportsAsync(FormAggregate form,
        IncidentReportsAggregateFilter req,
        CancellationToken ct)
    {
        var incidentReports = await context.IncidentReports
            .Include(x => x.Notes)
            .Include(x => x.Attachments)
            .Include(x => x.MonitoringObserver).ThenInclude(x => x.Observer).ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId)
            .Where(x => x.PollingStation != null
                        && (string.IsNullOrWhiteSpace(req.Level1Filter)
                            || EF.Functions.ILike(x.PollingStation.Level1, req.Level1Filter)))
            .Where(x => x.PollingStation != null
                        && (string.IsNullOrWhiteSpace(req.Level2Filter)
                            || EF.Functions.ILike(x.PollingStation.Level2, req.Level2Filter)))
            .Where(x => x.PollingStation != null
                        && (string.IsNullOrWhiteSpace(req.Level3Filter)
                            || EF.Functions.ILike(x.PollingStation.Level3, req.Level3Filter)))
            .Where(x => x.PollingStation != null
                        && (string.IsNullOrWhiteSpace(req.Level4Filter)
                            || EF.Functions.ILike(x.PollingStation.Level4, req.Level4Filter)))
            .Where(x => x.PollingStation != null
                        && (string.IsNullOrWhiteSpace(req.Level5Filter)
                            || EF.Functions.ILike(x.PollingStation.Level5, req.Level5Filter)))
            .Where(x => x.PollingStation != null
                        && (string.IsNullOrWhiteSpace(req.PollingStationNumberFilter)
                            || EF.Functions.ILike(x.PollingStation.Number, req.PollingStationNumberFilter)))
            .Where(x => req.HasFlaggedAnswers == null || (req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0))
            .Include(x => x.Form)
            .Where(x => req.QuestionsAnswered == null
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.All
                            && x.NumberOfQuestionsAnswered == x.Form.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.Some
                            && x.NumberOfQuestionsAnswered < x.Form.NumberOfQuestions)
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
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(ct);

        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var incidentReport in incidentReports)
        {
            formSubmissionsAggregate.AggregateAnswers(incidentReport);
        }

        var tasks = incidentReports.SelectMany(x => x.Attachments).Select(AttachmentModel.FromEntity)
            .Select(async attachment =>
            {
                var result =
                    await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
                if (result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds))
                {
                    return attachment with
                    {
                        PresignedUrl = url,
                        UrlValidityInSeconds = urlValidityInSeconds
                    };
                }

                return attachment;
            });

        var attachments = await Task.WhenAll(tasks);

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate,
            Notes = incidentReports.SelectMany(x => x.Notes).Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments,
            SubmissionsFilter = new SubmissionsFilterModel
            {
                HasAttachments = req.HasAttachments,
                HasNotes = req.HasNotes,
                Level1Filter = req.Level1Filter,
                Level2Filter = req.Level2Filter,
                Level3Filter = req.Level3Filter,
                Level4Filter = req.Level4Filter,
                Level5Filter = req.Level5Filter,
                QuestionsAnswered = req.QuestionsAnswered,
                HasFlaggedAnswers = req.HasFlaggedAnswers,
                IsCompletedFilter = req.IsCompletedFilter,
                LocationTypeFilter = req.LocationTypeFilter,
                FollowUpStatusFilter = req.FollowUpStatusFilter,
                PollingStationNumberFilter = req.PollingStationNumberFilter
            }
        });
    }
}
