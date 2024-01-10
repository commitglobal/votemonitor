using Vote.Monitor.Api.Feature.Forms.Update.Models;
using Vote.Monitor.Domain;

namespace Vote.Monitor.Api.Feature.Forms.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound>>
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
        Put("/api/election-rounds/{electionRoundId}/forms/{id}");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        _electionRoundIdProvider.SetElectionRound(req.ElectionRoundId);
        var form = await _repository.GetByIdAsync(req.Id, ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var mappedQuestions = req.Questions.ToEntities();
        form.UpdateDetails(req.LanguageId, req.Code, req.Description, mappedQuestions);

        await _repository.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}
