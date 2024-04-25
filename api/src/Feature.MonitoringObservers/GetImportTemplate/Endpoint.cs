using System.Text;
using Authorization.Policies;
using Vote.Monitor.Core.Extensions;

namespace Feature.MonitoringObservers.GetImportTemplate;

public class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/api/monitoring-observers:import-template");
        DontAutoTag();
        Options(x => x
            .WithTags("monitoring-observers")
            .Produces(200, typeof(string), contentType: "text/csv")
        );

        Summary(s =>
        {
            s.Summary = "Gets monitoring observers import template";
        });

        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        const string template = @"""Email"",""FirstName"",""LastName"",""PhoneNumber""" + "\n"
            + @"""alice@example.com"",""Alice"",""Smith"",""5551111""" + "\n"
            + @"""bob@example.com"",""Bob"",""Smith"",""5552222""";

        var stream = template.ToMemoryStream();
        await SendStreamAsync(stream, "import-template.csv", stream.Length, "text/csv", cancellation: ct);
    }
}
