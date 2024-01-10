using Vote.Monitor.Api.Feature.Forms.Models;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.Feature.Forms.Get;

public class Endpoint : Endpoint<Request, Results<Ok<FormModel>, NotFound>>
{
    private readonly IReadRepository<FormAggregate> _repository;
    private readonly IElectionRoundIdProvider _electionRoundIdProvider;

    public Endpoint(IReadRepository<FormAggregate> repository, IElectionRoundIdProvider electionRoundIdProvider)
    {
        _repository = repository;
        _electionRoundIdProvider = electionRoundIdProvider;
    }

    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<Ok<FormModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        _electionRoundIdProvider.SetElectionRound(req.ElectionRoundId);
        var form = await _repository.GetByIdAsync(req.Id, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

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
