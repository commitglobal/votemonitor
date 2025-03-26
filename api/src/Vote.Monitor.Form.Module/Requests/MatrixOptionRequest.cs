using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Requests;

public class MatrixOptionRequest
{
    public Guid Id { get; set; }
    public TranslatedString Text { get; set; }
    public bool IsFlagged { get; set; }
}
