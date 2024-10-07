using Ganss.Xss;

namespace Vote.Monitor.Core.Services.Security;

public class HtmlStringStringSanitizer : IHtmlStringSanitizer
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlStringStringSanitizer()
    {
        _sanitizer = new HtmlSanitizer();
        _sanitizer.AllowedSchemes.Add("mailto");
        _sanitizer.AllowedSchemes.Add("tel");
    }
    
    public string Sanitize(string html)
    {
        return _sanitizer.Sanitize(html);
    }
}