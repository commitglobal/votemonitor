﻿namespace Feature.PollingStation.Information.ListMy;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleForEach(x => x.PollingStationIds).NotEmpty();
    }
}
