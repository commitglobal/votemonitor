using Vote.Monitor.Core.Services.Time;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.InformationForm.Create;

public class Endpoint(IRepository<PollingStationInfoForm> repository, ITimeProvider timeProvider) :
        Endpoint<Request, Results<Ok<PollingStationInformationFormModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-station-information-form");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information-form"));
    }

    public override async Task<Results<Ok<PollingStationInformationFormModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
