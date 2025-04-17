namespace Feature.DataCleanup.DeleteNgoData;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid NgoId { get; set; }
}
