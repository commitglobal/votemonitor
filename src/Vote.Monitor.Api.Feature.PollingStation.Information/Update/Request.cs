using Vote.Monitor.Answer.Module.Requests;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Update;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    public Guid PollingStationId { get; set; }

    [FromClaim("Sub")]
    public Guid ObserverId { get; set; }
    public Guid FormId { get; set; }

    public Guid Id { get; set; }
    public List<BaseAnswerRequest> Answers { get; set; } = [];
}
