﻿using Vote.Monitor.Core.Security;

namespace Vote.Monitor.Api.Feature.Answers.Attachments.List;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }

    [FromClaim(ApplicationClaimTypes.UserId)]
    public Guid ObserverId { get; set; }
}