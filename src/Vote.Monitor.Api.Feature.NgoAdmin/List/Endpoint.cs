using Vote.Monitor.Api.Feature.NgoAdmin.Specifications;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Api.Feature.NgoAdmin.List;

public class Endpoint : Endpoint<Request, Results<Ok<PagedResponse<NgoAdminModel>>, ProblemDetails>>
{
     readonly IReadRepository<Domain.Entities.NgoAdminAggregate.NgoAdmin> _repository;

    public Endpoint(IReadRepository<Domain.Entities.NgoAdminAggregate.NgoAdmin> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/ngos/{ngoId}/admins");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
    }

    public override async Task<Results<Ok<PagedResponse<NgoAdminModel>>, ProblemDetails>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new ListNgoAdminsSpecification(req);
        var admins = await _repository.ListAsync(specification, ct);
        var adminsCount = await _repository.CountAsync(specification, ct);

        var result = admins.Select(x => new NgoAdminModel
        {
            Id = x.Id,
            FirstName = x.ApplicationUser.FirstName,
            LastName = x.ApplicationUser.LastName,
            Email = x.ApplicationUser.Email!,
            Status = x.ApplicationUser.Status,
            CreatedOn = x.CreatedOn,
            LastModifiedOn = x.LastModifiedOn
        }).ToList();

        return TypedResults.Ok(new PagedResponse<NgoAdminModel>(result, adminsCount, req.PageNumber, req.PageSize));
    }
}
