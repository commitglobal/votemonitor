using Feature.CitizenReports.GetSubmissionsAggregated;
using Module.Answers.Aggregators;
using Vote.Monitor.Core.Services.FileStorage.Contracts;
using AttachmentModel = Feature.CitizenReports.Models.AttachmentModel;
using NoteModel = Feature.CitizenReports.Models.NoteModel;

namespace Feature.CitizenReports.GetSubmissionsAggregatedV2;

public class Endpoint(
    VoteMonitorContext context,
    IAuthorizationService authorizationService,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/forms/{formId}:aggregated-submissionsV2");
        DontAutoTag();
        Options(x => x.WithTags("citizen-reports"));
        Policies(PolicyNames.NgoAdminsOnly);
        Summary(s =>
        {
            s.Summary =
                "Gets aggregated citizen report form submissions with all the notes and attachments (v2)";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult =
            await authorizationService.AuthorizeAsync(User,
                new CitizenReportingNgoAdminRequirement(req.ElectionRoundId));
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

        var citizenReports = await context.CitizenReports
            .Include(x => x.Notes)
            .Include(x => x.Attachments)
            .Include(x => x.Form)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId)
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
            SubmissionsFilter = new SubmissionsFilterModel()
        });
    }
}
