﻿using Vote.Monitor.Core.Validators;

namespace Vote.Monitor.Api.Feature.Answers.Attachments.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.PollingStationId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.ObserverId).NotEmpty();
        RuleFor(x => x.Attachment).FileSmallerThan(512 * 1024 * 1024); // 500 MB upload limit
    }
}

