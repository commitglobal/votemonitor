using Feature.IncidentsReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IncidentsReports.Attachments.List;

public class Endpoint(
    IReadRepository<IncidentReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/incident-report-attachments");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Gets all attachments an observer has uploaded for a incident report form";
        });
    }

    public override async Task<Response>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListIncidentReportAttachmentsSpecification(req.ElectionRoundId, req.IncidentReportId, req.FormId);
        var attachments = await repository.ListAsync(specification, ct);

        var tasks = attachments
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

                return new IncidentReportAttachmentModel
                {
                    FileName = attachment.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = attachment.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    Id = attachment.Id,
                    ElectionRoundId = attachment.ElectionRoundId,
                    IncidentReportId = attachment.IncidentReportId,
                    FormId = attachment.FormId,
                    QuestionId = attachment.QuestionId,
                };
            });

        var result = await Task.WhenAll(tasks);

        return new Response()
        {
            Attachments = result
        };
    }
}