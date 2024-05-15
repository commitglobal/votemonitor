namespace SubmissionsFaker.Clients.MonitoringObserver.Models;

public class SingleSelectAnswerRequest : BaseAnswerRequest
{
    public SelectedOptionRequest Selection { get; set; }
}
