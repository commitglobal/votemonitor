using Feature.CitizenReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.CitizenReports.Attachments.List;

public class Endpoint(
    IReadRepository<CitizenReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/attachments");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s =>
        {
            s.Summary = "Gets all attachments an observer has uploaded for a citizen report form";
        });
    }

    public override async Task<Response>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListCitizenReportAttachmentsSpecification(req.ElectionRoundId, req.CitizenReportId, req.FormId);
        var attachments = await repository.ListAsync(specification, ct);

        var tasks = attachments
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

                return new CitizenReportAttachmentModel
                {
                    FileName = attachment.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = attachment.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    Id = attachment.Id,
                    ElectionRoundId = attachment.ElectionRoundId,
                    CitizenReportId = attachment.CitizenReportId,
                    FormId = attachment.FormId,
                    QuestionId = attachment.QuestionId,
                };
            });

        var result = await Task.WhenAll(tasks);

        return new Response
        {
            Attachments = result
        };
    }
}