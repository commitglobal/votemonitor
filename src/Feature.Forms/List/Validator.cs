﻿namespace Feature.Forms.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.MonitoringNgoId).NotEmpty();

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
