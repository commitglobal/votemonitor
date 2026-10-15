using Vote.Monitor.Core.Models;

namespace Feature.Forms.Models;

public class NgoFormsResponseModel : CachedResponse
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<FormFullModel> Forms { get; set; } = [];
}
