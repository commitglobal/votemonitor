﻿using Feature.CitizenReports.Requests;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using AttachmentModel = Feature.CitizenReports.Models.AttachmentModel;
using NoteModel = Feature.CitizenReports.Models.NoteModel;

namespace Feature.CitizenReports.GetSubmissionsAggregated;

public class Endpoint(
    VoteMonitorContext context,
    IAuthorizationService authorizationService,
    IFileStorageService fileStorageService)
    : Endpoint<CitizenReportsAggregateFilter, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports/forms/{formId}:aggregated-submissions");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary =
                "Gets aggregated citizen report form submissions aggregated with all the notes and attachments";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(CitizenReportsAggregateFilter req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User, new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
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
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return await AggregateCitizenReportsAsync(form, req, ct);
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregateCitizenReportsAsync(FormAggregate form,
        CitizenReportsAggregateFilter req,
        CancellationToken ct)
    {
        var citizenReports = await context.CitizenReports
            .Include(x => x.Notes)
            .Include(x => x.Attachments)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId)
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
            .Include(x => x.Form)
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
            .AsSplitQuery()
            .AsNoTracking()
            .ToListAsync(ct);

        var formSubmissionsAggregate = new CitizenReportFormSubmissionsAggregate(form);
        foreach (var citizenReport in citizenReports)
        {
            formSubmissionsAggregate.AggregateAnswers(citizenReport);
        }

        var tasks = citizenReports.SelectMany(x => x.Attachments).Select(AttachmentModel.FromEntity)
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
            Notes = citizenReports.SelectMany(x => x.Notes).Select(NoteModel.FromEntity).ToArray(),
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
                HasFlaggedAnswers = req.HasFlaggedAnswers,
                QuestionsAnswered = req.QuestionsAnswered,
                FollowUpStatus = req.FollowUpStatus
            }
        });
    }
}
