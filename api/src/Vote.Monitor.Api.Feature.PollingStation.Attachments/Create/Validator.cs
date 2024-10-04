﻿using Vote.Monitor.Core.Validators;

namespace Vote.Monitor.Api.Feature.PollingStation.Attachments.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Attachment)
            .NotNull()
            .NotEmpty()
            .FileSmallerThan(512 * 1024 * 1024); // 500 MB upload limit
    }
}
