﻿using Vote.Monitor.Core.Security;

namespace Feature.Form.Submissions.GetAggregated;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid FormId { get; set; }

    [QueryParam] public string? Level1Filter { get; set; }

    [QueryParam] public string? Level2Filter { get; set; }

    [QueryParam] public string? Level3Filter { get; set; }

    [QueryParam] public string? Level4Filter { get; set; }

    [QueryParam] public string? Level5Filter { get; set; }

    [QueryParam] public string? PollingStationNumberFilter { get; set; }

    [QueryParam] public bool? HasFlaggedAnswers { get; set; }

    [QueryParam] public SubmissionFollowUpStatus? FollowUpStatus { get; set; }

    [QueryParam] public string[]? TagsFilter { get; set; } = [];
}