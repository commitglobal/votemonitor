using Vote.Monitor.Core.Models;

namespace Module.Forms.Requests;

public class SelectOptionRequest
{
    public Guid Id { get; set; }
    public TranslatedString Text { get; set; }
    public bool IsFlagged { get; set; }
    public bool IsFreeText { get; set; }
}
