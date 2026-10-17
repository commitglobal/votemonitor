using Vote.Monitor.Core.RulesEngine;
using Vote.Monitor.Core.RulesEngine.Rules;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.CitizenReportAggregate;

namespace Feature.CitizenReports.GetSubmissionsAggregatedV2;

public class Request : BaseFilterConditionsRequest
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
    [QueryParam] public Guid? LocationId { get; set; }
    [QueryParam] public bool? HasFlaggedAnswers { get; set; }
    [QueryParam] public CitizenReportFollowUpStatus? FollowUpStatus { get; set; }
    [QueryParam] public bool? HasNotes { get; set; }
    [QueryParam] public bool? HasAttachments { get; set; }
    [QueryParam] public QuestionsAnsweredFilter? QuestionsAnswered { get; set; }

    /// <summary>
    /// <see cref="BaseFilterConditionsRequest.Filter"/> combined with the query parameters.
    /// </summary>
    public FilterRule? ToFilterRule() => FilterRules.And(
        Filter,
        FilterRules.Eq("level1", Level1Filter),
        FilterRules.Eq("level2", Level2Filter),
        FilterRules.Eq("level3", Level3Filter),
        FilterRules.Eq("level4", Level4Filter),
        FilterRules.Eq("level5", Level5Filter),
        FilterRules.Eq("locationId", LocationId),
        FilterRules.Eq("hasFlaggedAnswers", HasFlaggedAnswers),
        FilterRules.Eq("followUpStatus", FollowUpStatus?.Name),
        FilterRules.Eq("hasNotes", HasNotes),
        FilterRules.Eq("hasAttachments", HasAttachments),
        FilterRules.Eq("questionsAnswered", QuestionsAnswered?.Name));
}
