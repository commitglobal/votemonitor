using Authorization.Policies;
using Feature.Form.Submissions.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;

namespace Feature.Form.Submissions.GetAggregated;

public class Endpoint(VoteMonitorContext context, IFileStorageService fileStorageService) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{formId}:aggregated");
        DontAutoTag();
        Options(x => x.WithTags("form-submissions", "mobile"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary = "Gets aggregated form with all the notes and attachments";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var form = await context
            .Forms
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringNgo.NgoId == req.NgoId
                        && x.Id == req.FormId)
            .FirstOrDefaultAsync(ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var submissions = await context.FormSubmissions
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.MonitoringObserver.MonitoringNgo.NgoId == req.NgoId
                        && x.FormId == req.FormId)
            .ToListAsync(ct);


        var formSubmissionsAggregate = new FormSubmissionsAggregate(form);
        foreach (var formSubmission in submissions)
        {
            formSubmissionsAggregate.AggregateAnswers(formSubmission);
        }

        var notes = await context
            .Notes
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.FormId == req.FormId)
            .Select(x => new NoteModel
            {
                MonitoringObserverId = x.MonitoringObserverId,
                QuestionId = x.QuestionId,
                Text = x.Text,
                TimeSubmitted = x.CreatedOn
            })
            .AsNoTracking()
            .ToListAsync(ct);

        var attachments = await context
            .Attachments
            .Where(x => x.ElectionRoundId == req.ElectionRoundId && x.FormId == req.FormId)
            .Select(x => new AttachmentModel
            {
                MonitoringObserverId = x.MonitoringObserverId,
                QuestionId = x.QuestionId,
                TimeSubmitted = x.CreatedOn,
                FileName = x.FileName,
                MimeType = x.MimeType,
                FilePath = x.FilePath,
                UploadedFileName = x.UploadedFileName
            })
            .AsNoTracking()
            .ToListAsync(ct);

        foreach (var attachment in attachments)
        {
            var result = await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName, ct);
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
}
