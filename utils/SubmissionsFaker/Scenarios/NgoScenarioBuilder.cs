using Spectre.Console;
using SubmissionsFaker.Extensions;
using SubmissionsFaker.Models;

namespace SubmissionsFaker.Scenarios;

public class NgoScenarioBuilder
{
    private readonly Dictionary<string, HttpClient> _admins = new();
    private readonly HttpClient _platformAdmin;
    private readonly Func<HttpClient> _clientFactory;
    public Guid NgoId { get; }

    public NgoScenarioBuilder(HttpClient platformAdmin,
        Func<HttpClient> clientFactory,
        Guid ngoId)
    {
        _platformAdmin = platformAdmin;
        _clientFactory = clientFactory;
        NgoId = ngoId;
    }

    public NgoScenarioBuilder WithAdmin(string? adminEmail = null)
    {
        adminEmail ??= Guid.NewGuid().ToString("N");
        var realEmail = $"{Guid.NewGuid()}@example.org";
        _platformAdmin.PostWithResponse<ResponseWithId>($"/api/ngos/{NgoId}/admins",
            new { FirstName = "NgoAdmin", LastName = adminEmail, Email = realEmail, Password = "string" });

        var adminClient = _clientFactory.NewForAuthenticatedUser(realEmail, "string");
        
        AnsiConsole.WriteLine($"Ngo admin created: {realEmail}");

        _admins.Add(adminEmail, adminClient);
        return this;
    }
    
    public HttpClient Admin => _admins.First().Value;

    public HttpClient AdminByName(string name) => _admins[name];
}
