using Vote.Monitor.Api.Feature.Forms.Models;
using Vote.Monitor.Api.Feature.Forms.Specifications;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain;
namespace Vote.Monitor.Api.Feature.Forms.List;

public class Endpoint : Endpoint<Request, PagedResponse<FormModel>>
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
        Get("/api/election-rounds/{electionRoundId}/forms");
        DontAutoTag();
        Options(x => x.WithTags("forms"));
    }

    public override async Task<PagedResponse<FormModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        _electionRoundIdProvider.SetElectionRound(req.ElectionRoundId);

        var specification = new ListFormsSpecification(req.CodeFilter, req.LanguageId, req.Status, req.PageSize, req.PageNumber);
        var forms = await _repository.ListAsync(specification, ct);
        var formsCount = await _repository.CountAsync(specification, ct);

        var result = forms.Select(x => new FormModel
        {
            Id = x.Id,
            LanguageId = x.LanguageId,
            Code = x.Code,
            Description = x.Description,
            Status = x.Status,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            Questions = x.Questions.ToModels()
        }).ToList();

        return new PagedResponse<FormModel>(result, formsCount, req.PageNumber, req.PageSize);
    }
}
