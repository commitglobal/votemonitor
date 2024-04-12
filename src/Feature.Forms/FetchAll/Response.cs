namespace Feature.Forms.FetchAll;

public class Response
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<FormFullModel> Forms { get; set; } = [];
}
