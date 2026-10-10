using Feature.IncidentReports.GetSubmissionsAggregated;
using Module.Answers.Aggregators;
using Vote.Monitor.Domain.ConnectionFactory;
using AttachmentModel = Feature.IncidentReports.Models.AttachmentModel;
using NoteModel = Feature.IncidentReports.Models.NoteModel;

namespace Feature.IncidentReports.GetSubmissionsAggregatedV2;

public class Endpoint(
    IAuthorizationService authorizationService,
    VoteMonitorContext context,
    IFileStorageService fileStorageService,
    INpgsqlConnectionFactory dbConnectionFactory)
    : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports/forms/{formId}:aggregated-submissionsV2");
        DontAutoTag();
        Options(x => x.WithTags("incident-reports"));
        Policies(PolicyNames.NgoAdminOrStaff);
        Summary(s =>
        {
            s.Summary = "Gets aggregated incident report form submissions with all the notes and attachments (v2)";
        });
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

        var filteredIds = req.Filter is null
            ? null
            : await IncidentReportFilterQuery.GetMatchingIdsAsync(
                dbConnectionFactory, req.ElectionRoundId, req.NgoId, req.Filter, ct);

        var incidentReports = await context.IncidentReports
            .Include(x => x.Notes)
            .Include(x => x.Attachments)
            .Include(x => x.MonitoringObserver).ThenInclude(x => x.Observer).ThenInclude(x => x.ApplicationUser)
            .Include(x => x.Form)
            .Where(x => x.ElectionRoundId == req.ElectionRoundId
                        && x.Form.MonitoringNgo.NgoId == req.NgoId
                        && x.Form.MonitoringNgo.ElectionRoundId == req.ElectionRoundId
                        && x.Form.ElectionRoundId == req.ElectionRoundId
                        && x.FormId == req.FormId
                        && (filteredIds == null || filteredIds.Contains(x.Id)))
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
            SubmissionsFilter = new SubmissionsFilterModel()
        });
    }
}
