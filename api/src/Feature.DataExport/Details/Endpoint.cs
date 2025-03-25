using Authorization.Policies;
using Feature.DataExport.Specifications;
using Vote.Monitor.Domain.Entities.ExportedDataAggregate;

namespace Feature.DataExport.Details;

public class Endpoint(IReadRepository<ExportedData> repository) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/exported-data/{id}:details");
        DontAutoTag();
        Options(x => x.WithTags("exported-data"));
        Summary(s =>
        {
            s.Summary = "Gets details about an exported data";
        });

        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var specification = new GetExportedDataDetailsSpecification(req.Id, req.UserId);

        var exportedDataDetails = await repository.SingleOrDefaultAsync(specification, ct);

        if (exportedDataDetails != null)
        {
            return TypedResults.Ok(exportedDataDetails);
        }

        return TypedResults.NotFound();

    }
}
