using Feature.Form.Submissions.GetAggregated;
using Feature.Form.Submissions.ListEntries;
using Microsoft.EntityFrameworkCore;
using Module.Answers.Aggregators;
using Module.Answers.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Feature.Form.Submissions.GetAggregatedV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    INpgsqlConnectionFactory connectionFactory,
    IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-submissions/{formId}:aggregatedV2");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Summary(s => { s.Summary = "Gets aggregated form with all the notes and attachments (v2)"; });
        Policies(PolicyNames.NgoAdminOrStaff);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
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

        if (psiForm is not null && psiForm.Id == req.FormId)
        {
            return await AggregatePSIFormSubmissionsAsync(psiForm, req, ct);
        }

        return TypedResults.NotFound();
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregateNgoFormSubmissionsAsync(FormAggregate form,
        Request req,
        CancellationToken ct)
    {
        var compiler = new FilterSqlCompiler(SubmissionFilterFields.All);
        var filter = compiler.Build(req.Filter);

        var parameters = new DynamicParameters();
        parameters.Add("electionRoundId", req.ElectionRoundId);
        parameters.Add("ngoId", req.NgoId);
        parameters.Add("dataSource", req.DataSource.ToString());

        filter.AddTo(parameters);

        var selectSql = $"""
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
                                s."MonitoringObserverStatus" AS "Status",
                                s."Tags",
                                s."NgoName",
                                s."NumberOfQuestionsAnswered",
                                s."NumberOfFlaggedAnswers",
                                s."MediaFilesCount",
                                s."NotesCount",
                                s."CommentsCount",
                                s."HasComments",
                                s."HasNotes",
                                s."HasAttachments",
                                s."FollowUpStatus",
                                s."IsCompleted",
                                s."MonitoringObserverStatus",
                                s."Answers"
                         FROM "GetFormSubmissionEntries"(@electionRoundId, @ngoId, @dataSource) s
                         WHERE {filter.Sql}
                         """;


        using var dbConnection = await connectionFactory.GetOpenConnectionAsync(ct);
        var submissions = (await dbConnection.QueryAsync<FormSubmissionView>(selectSql, parameters)).ToList();
        
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
            .SelectMany(submission => submission.Attachments.Select(attachment =>
                new AggregatedSubmissionsAttachmentModel
                {
                    SubmissionId = submission.SubmissionId,
                    QuestionId = attachment.QuestionId,
                    TimeSubmitted = attachment.TimeSubmitted,
                    MimeType = attachment.MimeType,
                    MonitoringObserverId = submission.MonitoringObserverId,
                    FilePath = attachment.FilePath,
                    UploadedFileName = attachment.UploadedFileName,
                    FileName = attachment.FileName,
                }));

        attachments = await Task.WhenAll(
            attachments.Select(async attachment =>
            {
                var result =
                    await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);
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
                DataSource = req.DataSource
            }
        });
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregatePSIFormSubmissionsAsync(
        PollingStationInformationForm form,
        Request req,
        CancellationToken ct)
    {
        var submissions = await context.PollingStationInformation
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId)
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
            Attachments = [],
            SubmissionsFilter = new SubmissionsFilterModel
            {
                DataSource = req.DataSource
            }
        });
    }
}
