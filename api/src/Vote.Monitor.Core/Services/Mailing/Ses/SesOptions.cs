namespace Vote.Monitor.Core.Services.Mailing.Ses;

public class SesOptions
{
    public const string SectionName = "Ses";
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }
}
