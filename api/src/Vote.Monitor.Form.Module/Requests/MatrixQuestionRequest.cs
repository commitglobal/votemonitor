namespace Vote.Monitor.Form.Module.Requests;

public class MatrixQuestionRequest: BaseQuestionRequest
{
    public List<MatrixOptionRequest> Options { get; set; } = new();
    public List<MatrixRowRequest> Rows { get; set; } = new();
}
