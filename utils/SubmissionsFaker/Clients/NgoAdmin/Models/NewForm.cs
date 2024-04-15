namespace SubmissionsFaker.Clients.NgoAdmin.Models;

public class NewForm
{
    public string Code { get; set; }
    public TranslatedString Name { get; set; }
    /// <summary>
    /// Opening | Voting | ClosingAndCounting
    /// </summary>
    public string FormType { get; set; }
    public List<string> Languages { get; set; } = [];
    public string DefaultLanguage { get; set; }
}