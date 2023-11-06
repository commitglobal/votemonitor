﻿using Vote.Monitor.Api.Feature.CSO.Specifications;

namespace Vote.Monitor.Api.Feature.CSO.Update;

public class Endpoint : Endpoint<Request, Results<NoContent, NotFound, Conflict<ProblemDetails>>>
{
    private readonly IRepository<Domain.Entities.CSOAggregate.CSO> _repository;

    public Endpoint(IRepository<Domain.Entities.CSOAggregate.CSO> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Put("/api/csos/{id}");
    }

    public override async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var CSO = await _repository.GetByIdAsync(req.Id, ct);

        if (CSO is null)
        {
            return TypedResults.NotFound();
        }

        var hasCSOWithSameName = await _repository.AnyAsync(new GetCSOByNameSpecification(req.Name), ct);

        if (hasCSOWithSameName)
        {
            AddError(r => r.Name, "A CSO with same name already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        CSO.UpdateDetails(req.Name);
        await _repository.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
