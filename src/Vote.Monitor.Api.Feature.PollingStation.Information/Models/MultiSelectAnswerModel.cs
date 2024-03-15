namespace Vote.Monitor.Api.Feature.PollingStation.Information.Models;

public class MultiSelectAnswerModel : BaseAnswerModel
{
    public List<SelectedOptionModel> Selection { get; set; }
}