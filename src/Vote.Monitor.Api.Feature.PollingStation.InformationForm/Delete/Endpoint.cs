using Feature.PollingStation.Information.Form.Specifications;

namespace Feature.PollingStation.Information.Form.Delete;

public class Endpoint(IRepository<PollingStationInfoFormAggregate> repository) : Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Delete("/api/election-rounds/{electionRoundId}/polling-station-information-form/");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information-form"));
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
       var pollingStationInformationForm  = await repository.FirstOrDefaultAsync(new GetPollingStationInformationFormSpecification(req.ElectionRoundId), ct);

       if (pollingStationInformationForm is null)
       {
           return TypedResults.NotFound();
       }

       await repository.DeleteAsync(pollingStationInformationForm, ct);
       return TypedResults.NoContent();
    }
}
