using Module.Forms.Requests;

namespace Feature.PollingStation.Information.Forms.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public string DefaultLanguage { get; set; }
    public List<string> Languages { get; set; } = new();
    public List<BaseQuestionRequest> Questions { get; set; } = new();

    /// <summary>
    /// The version the edit was made from (concurrent edits contract): a newer one on the server is a 409 and nothing
    /// is saved. Left out: saved without a check.
    /// </summary>
    public uint? Version { get; set; }
}
