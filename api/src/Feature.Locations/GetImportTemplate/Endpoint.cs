using System.Text;

namespace Feature.Locations.GetImportTemplate;

public class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/locations:import-template");
        DontAutoTag();
        Options(x => x.WithTags("locations"));

        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        const string template = """
                                "Level1", "Level2", "Level3", "Level4", "Level5", "DisplayOrder"
                                """;

        var stream = GenerateStreamFromString(template);
        await SendStreamAsync(stream, "import-template.csv", stream.Length, "text/csv", cancellation: ct);
    }

    private static MemoryStream GenerateStreamFromString(string value)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(value));
    }
}
