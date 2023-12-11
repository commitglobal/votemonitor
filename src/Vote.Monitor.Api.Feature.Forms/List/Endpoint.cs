using Vote.Monitor.Api.Feature.Forms.Models;
using Vote.Monitor.Api.Feature.Forms.Specifications;
using Vote.Monitor.Core.Models;
namespace Vote.Monitor.Api.Feature.Forms.List;

public class Endpoint : Endpoint<Request, PagedResponse<FormModel>>
{
    readonly IReadRepository<FormAggregate> _repository;

    public Endpoint(IReadRepository<FormAggregate> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        //Get("/api/election-rounds/{electionRoundId}/forms");
        Get("/api/forms");
        AllowAnonymous();
    }

    public override async Task<PagedResponse<FormModel>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListFormsSpecification(req.CodeFilter, req.LanguageFilter, req.Status, req.PageSize, req.PageNumber);
        var forms = await _repository.ListAsync(specification, ct);
        var formsCount = await _repository.CountAsync(specification, ct);
        var result = forms.Select(x => new FormModel
        {
            Id = x.Id,
            Code = x.Code,
            LanguageId = x.LanguageId,
            Description = x.Description,
            Status = x.Status,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
            Questions = new List<BaseQuestionModel>(){}
        }).ToList();

        return new PagedResponse<FormModel>(result, formsCount, req.PageNumber, req.PageSize);
    }
}
