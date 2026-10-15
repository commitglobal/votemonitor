using Vote.Monitor.Core.Models;

namespace Feature.Statistics.GetPollingStationStatistics;

public class Response : CachedResponse
{
    public int NumberOfFormSubmissions { get; set; }
    public int NumberOfQuestionsAnswered { get; set; }
    public int NumberOfQuickReports { get; set; }
    public int NumberOfObservers { get; set; }
    public bool HasPollingStationInformation { get; set; }
}
