namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class MultiSelectAnswerRequest : BaseAnswerRequest
{
    public List<SelectedOptionRequest> Selection { get; set; } = [];
}
