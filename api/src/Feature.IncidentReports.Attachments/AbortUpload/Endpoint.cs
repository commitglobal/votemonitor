﻿using Feature.IncidentReports.Attachments.Specifications;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.IncidentReports.Attachments.AbortUpload;

public class Endpoint(IRepository<IncidentReportAttachmentAggregate> repository,
    IFileStorageService fileStorageService)
    : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/incident-reports/{incidentReportId}/attachments/{id}:abort");
        DontAutoTag();
        Options(x => x.WithTags("incident-report-attachments"));
        Summary(s =>
        {
            s.Summary = "Aborts the upload and marks the attachment as deleted";
        });
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetAttachmentByIdSpecification(req.ElectionRoundId, req.IncidentReportId, req.Id);
        var attachment = await repository.FirstOrDefaultAsync(specification, ct);

        if (attachment == null)
        {
            return TypedResults.NotFound();
        }

        attachment.Delete();
        await repository.UpdateAsync(attachment, ct);

        await fileStorageService.AbortUploadAsync(req.UploadId, attachment.FilePath, attachment.UploadedFileName, ct);

        return TypedResults.NoContent();
    }
}