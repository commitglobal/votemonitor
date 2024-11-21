using Feature.IncidentReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IncidentReports.Attachments.Get;

public class Endpoint(
    IReadRepository<IncidentReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<IncidentReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/attachments/{id}");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Gets an attachment";
            s.Description = "Gets an attachment with freshly generated presigned url";
        });
    }

    public override async Task<Results<Ok<IncidentReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var attachment =
            await repository.FirstOrDefaultAsync(
                new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IncidentReportId, req.Id), ct);
        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var presignedUrl =
            await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

        return TypedResults.Ok(new IncidentReportAttachmentModel
        {
            FileName = attachment.FileName,
            PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
            MimeType = attachment.MimeType,
            UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
            Id = attachment.Id,
            ElectionRoundId = attachment.ElectionRoundId,
            IncidentReportId = attachment.IncidentReportId,
            FormId = attachment.FormId,
            QuestionId = attachment.QuestionId
        });
    }
}