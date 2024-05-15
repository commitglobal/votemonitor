using System.Text;
using Authorization.Policies;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;

namespace Feature.ImportErrors.Get;

public class Endpoint(VoteMonitorContext context) : Endpoint<Request>
{
    public override void Configure()
    {
        Get("/api/import-errors/{id}");
        DontAutoTag();
        Options(x => x
            .WithTags("import-errors")
            .Produces(200, typeof(string), contentType: "text/csv")
        );

        Summary(s =>
        {
            s.Summary = "Gets file with import errors";
        });

        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var importValidationErrors = await context
            .ImportValidationErrors
            .Where(x => x.Id == req.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        var stream = GenerateStreamFromString(importValidationErrors.Data);
        await SendStreamAsync(stream, $"erros-{importValidationErrors.OriginalFileName}", stream.Length, "text/csv", cancellation: ct);
    }

    private static MemoryStream GenerateStreamFromString(string value)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
    }
}
