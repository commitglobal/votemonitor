using Feature.CitizenReports.Models;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Answer.Module.Aggregators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using Vote.Monitor.Domain;

namespace Feature.CitizenReports.GetSubmissionsAggregated;

public class Endpoint(VoteMonitorContext context, IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/election-rounds/{electionRoundId}/citizen-reports/forms/{formId}:aggregated-submissions");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s => { s.Summary = "Gets aggregated citizen report form with all the notes and attachments"; });
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

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return await AggregateNgoFormSubmissionsAsync(form, req.ElectionRoundId, req.NgoId, req.FormId, ct);
    }

    private async Task<Results<Ok<Response>, NotFound>> AggregateNgoFormSubmissionsAsync(FormAggregate form,
        Guid electionRoundId,
        Guid ngoId,
        Guid formId,
        CancellationToken ct)
    {
        var citizenReports = await context.CitizenReports
            .Include(x => x.Notes)
            .Include(x => x.Attachments)
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.Form.MonitoringNgo.NgoId == ngoId
                        && x.FormId == formId)
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

  var attachments=      await Task.WhenAll(tasks);

        return TypedResults.Ok(new Response
        {
            SubmissionsAggregate = formSubmissionsAggregate,
            Notes = citizenReports.SelectMany(x => x.Notes).Select(NoteModel.FromEntity).ToArray(),
            Attachments = attachments
        });
    }
}