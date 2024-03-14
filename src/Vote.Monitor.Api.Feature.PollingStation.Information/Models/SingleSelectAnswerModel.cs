namespace Vote.Monitor.Api.Feature.PollingStation.Information.Models;

public class SingleSelectAnswerModel : BaseAnswerModel
{
    public SelectedOptionModel Selection { get; set; }
}