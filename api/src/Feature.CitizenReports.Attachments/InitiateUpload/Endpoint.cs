using Feature.CitizenReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.CitizenReports.Attachments.InitiateUpload;

public class Endpoint(
    IRepository<CitizenReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound, Conflict>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/citizen-reports/{citizenReportId}/attachments:init");
        DontAutoTag();
        AllowAnonymous();
        Options(x => x.WithTags("citizen-report-attachments", "public"));
        Summary(s =>
        {
            s.Summary =
                "Creates an attachment for a citizen report and gets back details for uploading it in the file storage";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound, Conflict>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.CitizenReportId, req.Id);
        var existingAttachment = await repository.FirstOrDefaultAsync(specification, ct);
        if (existingAttachment != null)
        {
            return TypedResults.Conflict();
        }

        var uploadPath =
            $"elections/{req.ElectionRoundId}/citizen-reports/{req.CitizenReportId}/form/{req.FormId}/attachments";

        var attachment = CitizenReportAttachmentAggregate.CreateV2(req.Id,
            req.ElectionRoundId,
            req.CitizenReportId,
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