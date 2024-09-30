using Feature.IssueReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IssueReports.Attachments.InitiateUpload;

public class Endpoint(
    IRepository<IssueReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<Ok<Response>, NotFound, Conflict>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/issue-report-attachments:init");
        DontAutoTag();
        Options(x => x.WithTags("issue-report-attachments"));
        Summary(s =>
        {
            s.Summary =
                "Creates an attachment for a issue report and gets back details for uploading it in the file storage";
        });
    }

    public override async Task<Results<Ok<Response>, NotFound, Conflict>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IssueReportId, req.Id);
        var existingAttachment = await repository.FirstOrDefaultAsync(specification, ct);
        if (existingAttachment != null)
        {
            return TypedResults.Conflict();
        }

        var uploadPath =
            $"elections/{req.ElectionRoundId}/issue-reports/{req.IssueReportId}/form/{req.FormId}/attachments";

        var attachment = IssueReportAttachmentAggregate.CreateV2(req.Id,
            req.ElectionRoundId,
            req.IssueReportId,
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