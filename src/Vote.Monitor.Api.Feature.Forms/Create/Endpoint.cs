using Vote.Monitor.Api.Feature.Forms.Models;
using Vote.Monitor.Api.Feature.Forms.Specifications;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.Feature.Forms.Create;

public class Endpoint : Endpoint<Request, Results<Ok<FormModel>, Conflict<ProblemDetails>>>
{
    private readonly IRepository<FormAggregate> _repository;
    private readonly IElectionRoundIdProvider _electionRoundIdProvider;

    public Endpoint(IRepository<FormAggregate> repository, IElectionRoundIdProvider electionRoundIdProvider)
    {
        _repository = repository;
        _electionRoundIdProvider = electionRoundIdProvider;
    }

    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        _electionRoundIdProvider.SetElectionRound(req.ElectionRoundId);
        var specification = new GetFormByCodeAndLanguage(req.Code, req.LanguageId);
        var hasFormWithSameCodes = await _repository.AnyAsync(specification, ct);

        if (hasFormWithSameCodes)
        {
            AddError(r => r.Code, "A form with the same combination of form code and language code already exists.");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var form = new FormAggregate(req.ElectionRoundId, req.LanguageId, req.Code, req.Description);
        await _repository.AddAsync(form, ct);

        return TypedResults.Ok(new FormModel
        {
            Id = form.Id,
            Code = form.Code,
            LanguageId = form.LanguageId,
            Description = form.Description,
            Status = form.Status,
            CreatedAt = form.CreatedAt,
            UpdatedAt = form.UpdatedAt,
            Questions = new List<BaseQuestionModel>() { }
        });
    }
}
