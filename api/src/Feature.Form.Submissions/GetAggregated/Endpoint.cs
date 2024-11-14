using Feature.Form.Submissions.Models;
using Feature.Form.Submissions.Requests;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Form.Submissions.GetAggregated;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    INpgsqlConnectionFactory connectionFactory,
    IFileStorageService fileStorageService) : Endpoint<FormSubmissionsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{formId}:aggregated");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s => { s.Summary = "Gets aggregated form with all the notes and attachments"; });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(FormSubmissionsAggregateFilter req,
        CancellationToken ct)
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
                        && x.MonitoringNgo.NgoId == req.NgoId
                        && x.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.Id == req.FormId)
            .Where(x => x.Status == FormStatus.Published)
            .Where(x => x.FormType != FormType.CitizenReporting && x.FormType != FormType.IncidentReporting)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (form is not null)
        {
            return await AggregateNgoFormSubmissionsAsync(form, req, ct);
        }

        var psiForm = await context
            .PollingStationInformationForms
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .FirstOrDefaultAsync(ct);

        if (psiForm is not null)
        {
            return await AggregatePSIFormSubmissionsAsync(psiForm, req, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregateNgoFormSubmissionsAsync(FormAggregate form,
        FormSubmissionsAggregateFilter req,
        CancellationToken ct)
    {
        var tags = req.TagsFilter ?? [];
        var submissions = await context
            .FormSubmissions
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId)
            .Where(x => string.IsNullOrWhiteSpace(req.Level1Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level1, req.Level1Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level2Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level2, req.Level2Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level3Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level3, req.Level3Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level4Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level4, req.Level4Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level5Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level5, req.Level5Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.PollingStationNumberFilter) ||
                        EF.Functions.ILike(x.PollingStation.Number, req.PollingStationNumberFilter))
            .Where(x => req.HasFlaggedAnswers == null || (req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0))
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
            .Where(x => tags.Length == 0 || x.MonitoringObserver.Tags.Any(tag => tags.Contains(tag)))
            .Where(x => req.MonitoringObserverStatus == null ||
                        x.MonitoringObserver.Status == req.MonitoringObserverStatus)
            .Where(x => req.QuestionsAnswered == null
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.All &&
                            x.NumberOfQuestionsAnswered == x.Form.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.Some &&
                            x.NumberOfQuestionsAnswered < x.Form.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.None && x.NumberOfQuestionsAnswered == 0))
            .Where(x => req.HasNotes == null || (req.HasNotes.Value
                ? context.Notes.Count(n =>
                    n.MonitoringObserverId == x.MonitoringObserverId
                    && n.FormId == x.FormId
                    && x.Form.ElectionRoundId == req.ElectionRoundId
                    && n.PollingStationId == x.PollingStationId
                    && n.ElectionRoundId == x.ElectionRoundId) > 0
                : context.Notes.Count(n =>
                    n.MonitoringObserverId == x.MonitoringObserverId
                    && n.FormId == x.FormId
                    && x.Form.ElectionRoundId == req.ElectionRoundId
                    && n.PollingStationId == x.PollingStationId
                    && n.ElectionRoundId == x.ElectionRoundId) == 0))
            .Where(x => req.HasAttachments == null || (req.HasAttachments.Value
                ? context.Attachments.Count(a =>
                    a.MonitoringObserverId == x.MonitoringObserverId
                    && a.FormId == x.FormId
                    && x.Form.ElectionRoundId == req.ElectionRoundId
                    && a.PollingStationId == x.PollingStationId
                    && a.ElectionRoundId == x.ElectionRoundId) > 0
                : context.Attachments.Count(a =>
                    a.MonitoringObserverId == x.MonitoringObserverId
                    && a.FormId == x.FormId
                    && x.Form.ElectionRoundId == req.ElectionRoundId
                    && a.PollingStationId == x.PollingStationId
                    && a.ElectionRoundId == x.ElectionRoundId) == 0))
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync(ct);

        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var formSubmission in submissions)
        {
            formSubmissionsAggregate.AggregateAnswers(formSubmission);
        }

        var pollingStationIds = submissions.Select(x => x.PollingStationId).Distinct().ToArray();
        var monitoringObserverIds = submissions.Select(x => x.MonitoringObserverId).Distinct().ToArray();

        var sql = """
                  SELECT
                      N."Id",
                      N."MonitoringObserverId",
                      N."QuestionId",
                      N."Text",
                      COALESCE(N."LastModifiedOn", N."CreatedOn") "TimeSubmitted",
                      (
                          SELECT
                              FS."Id"
                          FROM
                              "FormSubmissions" FS
                          WHERE
                              FS."MonitoringObserverId" = N."MonitoringObserverId"
                              AND FS."FormId" = N."FormId"
                              AND FS."PollingStationId" = N."PollingStationId"
                              AND FS."ElectionRoundId" = N."ElectionRoundId"
                      ) "SubmissionId"
                  FROM
                      "Notes" N
                  WHERE
                      N."ElectionRoundId" = @electionRoundId
                      AND N."FormId" = @formId
                      AND N."MonitoringObserverId" = ANY (@monitoringObserverIds)
                      AND N."PollingStationId" = ANY (@pollingStationIds);

                  SELECT
                      A."MonitoringObserverId",
                      A."QuestionId",
                      A."FileName",
                      A."MimeType",
                      A."FilePath",
                      A."UploadedFileName",
                      COALESCE(A."LastModifiedOn", A."CreatedOn") "TimeSubmitted",
                      (
                          SELECT
                              FS."Id"
                          FROM
                              "FormSubmissions" FS
                          WHERE
                              FS."MonitoringObserverId" = A."MonitoringObserverId"
                              AND FS."FormId" = A."FormId"
                              AND FS."PollingStationId" = A."PollingStationId"
                              AND FS."ElectionRoundId" = A."ElectionRoundId"
                      ) "SubmissionId"
                  FROM
                      "Attachments" A
                  WHERE
                      A."ElectionRoundId" = @electionRoundId
                      AND A."IsDeleted" = false AND A."IsCompleted" = TRUE
                      AND A."FormId" = @formId
                      AND A."MonitoringObserverId" = ANY (@monitoringObserverIds)
                      AND A."PollingStationId" = ANY (@pollingStationIds);
                  """;

        var queryArgs = new
        {
            electionRoundId = req.ElectionRoundId,
            formId = req.FormId,
            monitoringObserverIds,
            pollingStationIds
        };

        List<NoteModel> notes;
        List<AttachmentModel> attachments;

        using (var dbConnection = await connectionFactory.GetOpenConnectionAsync(ct))
        {
            using var multi = await dbConnection.QueryMultipleAsync(sql, queryArgs);
            notes = multi.Read<NoteModel>().ToList();
            attachments = multi.Read<AttachmentModel>().ToList();
        }

        foreach (var attachment in attachments)
        {
            var result =
                await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
            if (result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds))
            {
                attachment.PresignedUrl = url;
                attachment.UrlValidityInSeconds = urlValidityInSeconds;
            }
        }

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate,
            Notes = notes,
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
                TagsFilter = req.TagsFilter,
                FollowUpStatus = req.FollowUpStatus,
                HasFlaggedAnswers = req.HasFlaggedAnswers,
                MonitoringObserverStatus = req.MonitoringObserverStatus,
                PollingStationNumberFilter = req.PollingStationNumberFilter
            }
        });
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregatePSIFormSubmissionsAsync(
        PollingStationInformationForm form,
        FormSubmissionsAggregateFilter req,
        CancellationToken ct)
    {
        var tags = req.TagsFilter ?? [];

        var submissions = await context.PollingStationInformation
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId)
            .Where(x => string.IsNullOrWhiteSpace(req.Level1Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level1, req.Level1Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level2Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level2, req.Level2Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level3Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level3, req.Level3Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level4Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level4, req.Level4Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.Level5Filter) ||
                        EF.Functions.ILike(x.PollingStation.Level5, req.Level5Filter))
            .Where(x => string.IsNullOrWhiteSpace(req.PollingStationNumberFilter) ||
                        EF.Functions.ILike(x.PollingStation.Number, req.PollingStationNumberFilter))
            .Where(x => req.HasFlaggedAnswers == null || (req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0))
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
            .Where(x => tags.Length == 0 || x.MonitoringObserver.Tags.Any(tag => tags.Contains(tag)))
            .Where(x => req.MonitoringObserverStatus == null ||
                        x.MonitoringObserver.Status == req.MonitoringObserverStatus)
            .Where(x => req.QuestionsAnswered == null
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.All &&
                            x.NumberOfQuestionsAnswered == x.PollingStationInformationForm.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.Some &&
                            x.NumberOfQuestionsAnswered < x.PollingStationInformationForm.NumberOfQuestions)
                        || (req.QuestionsAnswered == QuestionsAnsweredFilter.None && x.NumberOfQuestionsAnswered == 0))
            .Where(x => req.HasNotes == null || !req.HasNotes.Value)
            .Where(x => req.HasAttachments == null || !req.HasAttachments.Value)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync(ct);

        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var formSubmission in submissions)
        {
            formSubmissionsAggregate.AggregateAnswers(formSubmission);
        }

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate,
            Notes = [],
            Attachments = []
        });
    }
}
