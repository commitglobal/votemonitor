using Feature.Review;

namespace Feature.Severity.Set;

public class Request : ReportRequest
{
    public SeverityLevel Severity { get; set; } = null!;
}
