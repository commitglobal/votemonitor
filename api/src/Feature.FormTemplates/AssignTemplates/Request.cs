namespace Feature.FormTemplates.AssignTemplates;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public List<Guid> FormTemplateIds { get; set; } = new List<Guid>();
}
