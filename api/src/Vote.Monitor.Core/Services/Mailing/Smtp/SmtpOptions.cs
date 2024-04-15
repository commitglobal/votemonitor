namespace Vote.Monitor.Core.Services.Mailing.Smtp;

public class SmtpOptions
{
    public const string SectionName = "Smtp";
    public string SenderName { get; set; }
    public string SenderEmail { get; set; }

    public string Host { get; set; }

    public int Port { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
}
