using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;

namespace Feature.IncidentReports.GetQuestionAggregatedByLevel;

public class Request : BaseFilterConditionsRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public Guid FormId { get; set; }

    public Guid QuestionId { get; set; }

    /// <summary>
    /// The level to break the submissions down by.
    /// </summary>
    public LocationLevel Level { get; set; } = null!;
}
