namespace Feature.Forms.Models;

public class NgoFormsResponseModel
{
    public Guid ElectionRoundId { get; set; }
    public string Version { get; set; }
    public List<FormFullModel> Forms { get; set; } = [];
}
