namespace Feature.Form.Submission.SMS.Submit;
public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid MonitoringNgoId { get; set; }
    public string PhoneNumber { get; set; }
    public string SmsMessage { get; set; }
}
