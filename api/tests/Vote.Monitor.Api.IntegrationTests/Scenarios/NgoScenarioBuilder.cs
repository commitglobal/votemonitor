using Vote.Monitor.Api.IntegrationTests.Models;

namespace Vote.Monitor.Api.IntegrationTests.Scenarios;

public class NgoScenarioBuilder
{
    private readonly Dictionary<string, HttpClient> _admins = new();
    private readonly Dictionary<string, Guid> _adminIds = new();
    private readonly Dictionary<string, (Guid Id, HttpClient Client)> _staff = new();
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
        var admin = _platformAdmin.PostWithResponse<ResponseWithId>($"/api/ngos/{NgoId}/admins",
            new { FirstName = "Observer", LastName = adminEmail, Email = realEmail, Password = "string" });

        var adminClient = _clientFactory.NewForAuthenticatedUser(realEmail, "string");

        _admins.Add(adminEmail, adminClient);
        _adminIds.Add(adminEmail, admin.Id);
        return this;
    }

    public NgoScenarioBuilder WithStaff(string name)
    {
        var realEmail = $"{Guid.NewGuid()}@example.org";
        var staff = _platformAdmin.PostWithResponse<ResponseWithId>($"/api/ngos/{NgoId}/staff",
            new { FirstName = "Staff", LastName = name, Email = realEmail, Password = "string" });

        _staff.Add(name, (staff.Id, _clientFactory.NewForAuthenticatedUser(realEmail, "string")));
        return this;
    }

    public HttpClient Admin => _admins.First().Value;
    public Guid AdminId => _adminIds.First().Value;

    public HttpClient AdminByName(string name) => _admins[name];

    public HttpClient StaffByName(string name) => _staff[name].Client;
    public Guid StaffIdByName(string name) => _staff[name].Id;
}
