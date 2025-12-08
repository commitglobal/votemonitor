namespace Feature.Attachments.InitiateUploadV2;

public class Response
{
    public string UploadId { get; set; }
    public Dictionary<int, string> UploadUrls { get; set; }
}
