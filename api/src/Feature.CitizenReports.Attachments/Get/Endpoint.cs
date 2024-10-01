using Feature.CitizenReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.CitizenReports.Attachments.Get;

public class Endpoint(
    IReadRepository<CitizenReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<CitizenReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/citizen-report-attachments/{id}");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s =>
        {
            s.Summary = "Gets an attachment";
            s.Description = "Gets an attachment with freshly generated presigned url";
        });
    }

    public override async Task<Results<Ok<CitizenReportAttachmentModel>, BadRequest<ProblemDetails>, NotFound>>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var attachment =
            await repository.FirstOrDefaultAsync(
                new GetAttachmentByIdSpecification(req.ElectionRoundId, req.CitizenReportId, req.Id), ct);
        if (attachment is null)
        {
            return TypedResults.NotFound();
        }

        var presignedUrl =
            await fileStorageService.GetPresignedUrlAsync(attachment.FilePath, attachment.UploadedFileName);

        return TypedResults.Ok(new CitizenReportAttachmentModel
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
        });
    }
}