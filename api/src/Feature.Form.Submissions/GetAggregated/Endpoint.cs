using Feature.Form.Submissions.Requests;
using Microsoft.EntityFrameworkCore;
using Module.Answers.Aggregators;
using Module.Answers.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Queries;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
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
            .FromSqlInterpolated($"""
                                  select f.* from "GetAvailableForms"({req.ElectionRoundId}, {req.NgoId}, {req.DataSource.ToString()}) af
                                  inner join "Forms" f on f."Id" = af."FormId"
                                  """)
            .Where(x => x.Id == req.FormId)
            .Where(x => x.Status == FormStatus.Published)
            .Where(x => x.FormType != FormType.CitizenReporting)
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
        var builder = new SqlBuilder();
        builder.AddParameters(new
        {
            electionRoundId = req.ElectionRoundId,
            ngoId = req.NgoId,
            dataSource = req.DataSource.ToString()
        });

        builder.ApplyFilters(new FormSubmissionEntriesFilterCriteria
        {
            CoalitionMemberId = req.CoalitionMemberId,
            Level1 = req.Level1Filter,
            Level2 = req.Level2Filter,
            Level3 = req.Level3Filter,
            Level4 = req.Level4Filter,
            Level5 = req.Level5Filter,
            PollingStationNumber = req.PollingStationNumberFilter,
            PollingStationId = req.PollingStationId,
            HasFlaggedAnswers = req.HasFlaggedAnswers,
            FollowUpStatus = req.FollowUpStatus?.ToString(),
            Tags = req.TagsFilter,
            MonitoringObserverStatus = req.MonitoringObserverStatus?.ToString(),
            FormId = req.FormId,
            HasNotes = req.HasNotes,
            HasAttachments = req.HasAttachments,
            QuestionsAnswered = req.QuestionsAnswered?.ToString(),
            FromDate = req.FromDateFilter,
            ToDate = req.ToDateFilter
        });

        var template = builder.AddTemplate(
            """
            SELECT s."SubmissionId",
                   s."TimeSubmitted",
                   s."FormId",
                   s."FormCode",
                   s."FormType",
                   s."DefaultLanguage",
                   s."FormName",
                   s."PollingStationId",
                   s."Level1",
                   s."Level2",
                   s."Level3",
                   s."Level4",
                   s."Level5",
                   s."Number",
                   s."MonitoringObserverId",
                   s."DisplayName" AS "ObserverName",
                   s."Email",
                   s."PhoneNumber",
                   s."Tags",
                   s."NgoName",
                   s."NumberOfQuestionsAnswered",
                   s."NumberOfFlaggedAnswers",
                   s."FollowUpStatus",
                   s."IsCompleted",
                   s."IsOwnObserver",
                   s."Notes",
                   s."Attachments",
                   s."Answers"
            FROM "GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
            /**where**/
            """);

        List<FormSubmissionView> submissions;
        using (var dbConnection = await connectionFactory.GetOpenConnectionAsync(ct))
        {
            submissions = (await dbConnection.QueryAsync<FormSubmissionView>(template.RawSql, template.Parameters))
                .ToList();
        }

        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var formSubmission in submissions)
        {
            formSubmissionsAggregate.AggregateAnswers(formSubmission);
        }
        
        var notes = submissions
            .SelectMany(x => x.Notes.Select(note => note with
            {
                SubmissionId = x.SubmissionId,
                MonitoringObserverId = x.MonitoringObserverId
            }))
            .ToList();

        var attachments = submissions
            .SelectMany(submission => submission.Attachments.Select(attachment => new AggregatedSubmissionsAttachmentModel()
            {
                SubmissionId = submission.SubmissionId,
                QuestionId = attachment.QuestionId,
                TimeSubmitted = attachment.TimeSubmitted,
                MimeType = attachment.MimeType,
                MonitoringObserverId = submission.MonitoringObserverId,
                FilePath = attachment.FilePath,
                UploadedFileName = attachment.UploadedFileName,
                FileName = attachment.FileName,
            } ));

        attachments = await Task.WhenAll(
            attachments.Select(async attachment =>
            {
                var result = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
                return result is GetPresignedUrlResult.Ok(var url, _, var urlValidityInSeconds)
                    ? attachment with { PresignedUrl = url, UrlValidityInSeconds = urlValidityInSeconds }
                    : attachment;
            })
        );

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate,
            Notes = notes,
            Attachments = attachments.ToList(),
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
                PollingStationNumberFilter = req.PollingStationNumberFilter,
                PollingStationId = req.PollingStationId,
                DataSource = req.DataSource!,
                CoalitionMemberId = req.CoalitionMemberId,
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
            .Where(x => req.PollingStationId == null || x.PollingStationId == req.PollingStationId)
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
            SubmissionsAggregate = formSubmissionsAggregate, Notes = [], Attachments = []
        });
    }
}
