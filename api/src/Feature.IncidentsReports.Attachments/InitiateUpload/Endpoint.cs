using Feature.IncidentsReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IncidentsReports.Attachments.InitiateUpload;

public class Endpoint(
    IRepository<IncidentReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound, Conflict>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-report-attachments:init");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s =>
        {
            s.Summary =
                "Creates an attachment for a incident report and gets back details for uploading it in the file storage";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound, Conflict>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IncidentReportId, req.Id);
        var existingAttachment = await repository.FirstOrDefaultAsync(specification, ct);
        if (existingAttachment != null)
        {
            return TypedResults.Conflict();
        }

        var uploadPath =
            $"elections/{req.ElectionRoundId}/incident-reports/{req.IncidentReportId}/form/{req.FormId}/attachments";

        var attachment = IncidentReportAttachmentAggregate.CreateV2(req.Id,
            req.ElectionRoundId,
            req.IncidentReportId,
            req.FormId,
            req.QuestionId,
            req.FileName,
            uploadPath,
            req.ContentType);

        var uploadResult = await fileStorageService.CreateMultipartUploadAsync(uploadPath,
            fileName: attachment.UploadedFileName,
            contentType: req.ContentType,
            numberOfUploadParts: req.NumberOfUploadParts,
            ct: ct);

        await repository.AddAsync(attachment, ct);

        return TypedResults.Ok(new Response
        {
            UploadId = uploadResult.UploadId,
            UploadUrls = uploadResult.PresignedUrls
        });
    }
}