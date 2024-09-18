using Authorization.Policies;
using Dapper;
using Feature.Form.Submissions.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.ConnectionFactory;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Form.Submissions.GetAggregated;

public class Endpoint(
    VoteMonitorContext context,
    INpgsqlConnectionFactory connectionFactory,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{formId}:aggregated");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Gets aggregated form with all the notes and attachments"; });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var form = await context
            .Forms
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringNgo.NgoId == req.NgoId
                        && x.Id == req.FormId)
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
        Request req,
        CancellationToken ct)
    {
        var tags = req.TagsFilter ?? [];
        var submissions = await context
            .FormSubmissions
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Include(x => x.PollingStation)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
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
            .Where(x => req.HasFlaggedAnswers == null || req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0)
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
            .Where(x => tags.Length == 0 || x.MonitoringObserver.Tags.Any(tag => tags.Contains(tag)))
            .AsNoTracking()
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
            Attachments = attachments
        });
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregatePSIFormSubmissionsAsync(
        PollingStationInformationForm form,
        Request req,
        CancellationToken ct)
    {
        var tags = req.TagsFilter ?? [];

        var submissions = await context.PollingStationInformation
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
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
            .Where(x => req.HasFlaggedAnswers == null || req.HasFlaggedAnswers.Value
                ? x.NumberOfFlaggedAnswers > 0
                : x.NumberOfFlaggedAnswers == 0)
            .Where(x => req.FollowUpStatus == null || x.FollowUpStatus == req.FollowUpStatus)
            .Where(x => tags.Length == 0 || x.MonitoringObserver.Tags.Any(tag => tags.Contains(tag)))
            .AsNoTracking()
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