using Vote.Monitor.Api.Feature.Ngo.Specifications;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.Ngo.List;

public class Endpoint(IReadRepository<NgoAggregate> repository) : Endpoint<Request, Results<Ok<PagedResponse<NgoModel>>, ProblemDetails>>
{
    public override void Configure()
    {
        Get("/api/ngos");
    }

    public override async Task<Results<Ok<PagedResponse<NgoModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListNgosSpecification(req);
        var ngos = await repository.ListAsync(specification, ct);
        var ngosCount = await repository.CountAsync(specification, ct);

        var result = ngos.Select(x => new NgoModel
        {
            Id = x.Id,
            Name = x.Name,
            Status = x.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn
        }).ToList();

        return TypedResults.Ok(new PagedResponse<NgoModel>(result, ngosCount, req.PageNumber, req.PageSize));
    }
}
