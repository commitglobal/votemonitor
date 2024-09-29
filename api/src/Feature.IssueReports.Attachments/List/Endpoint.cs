using Feature.IssueReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IssueReports.Attachments.List;

public class Endpoint(
    IReadRepository<IssueReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/issue-report-attachments");
        DontAutoTag();
        Options(x => x.WithTags("issue-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Gets all attachments an observer has uploaded for a issue report form";
        });
    }

    public override async Task<Response>
        ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListIssueReportAttachmentsSpecification(req.ElectionRoundId, req.IssueReportId, req.FormId);
        var attachments = await repository.ListAsync(specification, ct);

        var tasks = attachments
            .Select(async attachment =>
            {
                var presignedUrl = await fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

                return new IssueReportAttachmentModel
                {
                    FileName = attachment.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = attachment.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    Id = attachment.Id,
                    ElectionRoundId = attachment.ElectionRoundId,
                    IssueReportId = attachment.IssueReportId,
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