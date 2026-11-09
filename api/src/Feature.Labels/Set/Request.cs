using Feature.Review;

namespace Feature.Labels.Set;

public class Request : ReportRequest
{
    /// <summary>The record's labels; replaces the ones it has.</summary>
    public string[] Labels { get; set; } = null!;
}
