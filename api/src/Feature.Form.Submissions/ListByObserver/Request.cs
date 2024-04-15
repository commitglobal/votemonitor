using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Form.Submissions.ListByObserver;

public class Request : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim("NgoId")]
    public Guid NgoId { get; set; }

    [QueryParam]
    public string? FormCodeFilter { get; set; }

    [QueryParam]
    public FormType? FormTypeFilter { get; set; }

    [QueryParam]
    public string? Level1Filter { get; set; }

    [QueryParam]
    public string? Level2Filter { get; set; }

    [QueryParam]
    public string? Level3Filter { get; set; }

    [QueryParam]
    public string? Level4Filter { get; set; }

    [QueryParam]
    public string? Level5Filter { get; set; }

    [QueryParam]
    public string? PollingStationNumberFilter { get; set; }

    [QueryParam]
    public bool? HasFlaggedAnswers { get; set; }

}
