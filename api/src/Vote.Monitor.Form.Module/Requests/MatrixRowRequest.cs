using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Form.Module.Requests;

public class MatrixRowRequest
{
    public Guid Id { get; set; }
    public TranslatedString Text { get; set; }
}
