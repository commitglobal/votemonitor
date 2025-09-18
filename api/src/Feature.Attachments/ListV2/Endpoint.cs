using Authorization.Policies.Requirements;
using Feature.Attachments.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.FileStorage.Contracts;

namespace Feature.Attachments.ListV2;

public class Endpoint : Endpoint<Request, Results<Ok<List<AttachmentModelV2>>, NotFound, BadRequest<ProblemDetails>>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IReadRepository<AttachmentAggregate> _repository;
    private readonly IFileStorageService _fileStorageService;

    public Endpoint(IAuthorizationService authorizationService,
        IReadRepository<AttachmentAggregate> repository,
        IFileStorageService fileStorageService)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/form-submissions/{submissionId}/attachments");
        DontAutoTag();
        Options(x => x.WithTags("attachments", "mobile"));
        Summary(s =>
        {
            s.Summary = "Gets all attachments an observer has uploaded for a submission";
            s.Description = "Gets all attachments with freshly generated presigned urls";
        });
    }

    public override async Task<Results<Ok<List<AttachmentModelV2>>, NotFound, BadRequest<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, new MonitoringObserverRequirement(req.ElectionRoundId));
        if (!authorizationResult.Succeeded)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetObserverAttachmentsSpecification(req.ElectionRoundId, req.SubmissionId, req.ObserverId);
        var attachments = await _repository.ListAsync(specification, ct);

        var tasks = attachments
            .Select(async attachment =>
            {
                var presignedUrl = await _fileStorageService.GetPresignedUrlAsync(
                    attachment.FilePath,
                    attachment.UploadedFileName);

                return new AttachmentModelV2
                {
                    Id = attachment.Id,
                    ElectionRoundId = req.ElectionRoundId,
                    FileName = attachment.FileName,
                    PresignedUrl = (presignedUrl as GetPresignedUrlResult.Ok)?.Url ?? string.Empty,
                    MimeType = attachment.MimeType,
                    UrlValidityInSeconds = (presignedUrl as GetPresignedUrlResult.Ok)?.UrlValidityInSeconds ?? 0,
                    SubmissionId = attachment.SubmissionId,
                    QuestionId = attachment.QuestionId
                };
            });

        var result = await Task.WhenAll(tasks);

        return TypedResults.Ok(result.ToList());
    }
}
