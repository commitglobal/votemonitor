using Vote.Monitor.Core.Models;

namespace Feature.PollingStation.Information.PsiData;

public class PsiDataFilters : BaseSortPaginatedRequest
{
    public Guid ElectionRoundId { get; set; }

    [QueryParam] public string? SearchText { get; set; }

    [QueryParam] public string? Level1Filter { get; set; }
    [QueryParam] public string? Level2Filter { get; set; }
    [QueryParam] public string? Level3Filter { get; set; }
    [QueryParam] public string? Level4Filter { get; set; }
    [QueryParam] public string? Level5Filter { get; set; }

    [QueryParam] public Guid? MonitoringNgoId { get; set; }

    [QueryParam] public bool? IsCompleted { get; set; }
    [QueryParam] public DateTime? FromDateFilter { get; set; }
    [QueryParam] public DateTime? ToDateFilter { get; set; }
}
