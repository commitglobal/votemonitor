﻿namespace Feature.PollingStation.Information.Form.Get;

public class Endpoint(IReadRepository<PollingStationInfoFormAggregate> repository) : Endpoint<Request, Results<Ok<PollingStationInformationFormModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/polling-station-information-form");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information-form", "mobile"));
        Description(x => x.Accepts<Request>()); Summary(s => {
            s.Summary = "Gets the polling station information form.";
            s.Description = "Gets the polling station information form. Observers will fill in this form when they visit a polling station.";
        });
    }

    public override async Task<Results<Ok<PollingStationInformationFormModel>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var form = await repository.FirstOrDefaultAsync(new PollingStationInformationModelSpecification(req.ElectionRoundId), ct);

        if (form is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(form);
    }
}
