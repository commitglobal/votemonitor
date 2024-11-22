using Job.Contracts;
using NSubstitute;
using NSubstitute.ClearExtensions;
using Vote.Monitor.Api.IntegrationTests.Db;
using Vote.Monitor.Core.Services.EmailTemplating;
using Vote.Monitor.Core.Services.EmailTemplating.Props;
using Vote.Monitor.Core.Services.Time;

namespace Vote.Monitor.Api.IntegrationTests;

[SetUpFixture]
public class ApiTesting
{
    private static ITestDatabase _database = null!;
    private static CustomWebApplicationFactory _factory = null!;
    private static ITimeProvider _apiTimeProvider = null!;
    public static ITimeProvider ApiTimeProvider => _apiTimeProvider;

    private static IEmailTemplateFactory _emailFactory = null!;
    public static IEmailTemplateFactory EmailFactory => _emailFactory;
    private static IJobService _jobService = null!;
    public static IJobService JobService => _jobService;

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();

        _apiTimeProvider = Substitute.For<ITimeProvider>();
        _apiTimeProvider.UtcNow.Returns(_ => DateTime.UtcNow);
        _apiTimeProvider.UtcNowDate.Returns(_ => DateOnly.FromDateTime(DateTime.UtcNow));

        _emailFactory = Substitute.For<IEmailTemplateFactory>();
        _emailFactory.GenerateConfirmAccountEmail(Arg.Any<ConfirmEmailProps>())
            .Returns(new EmailModel("fake", "fake"));

        _emailFactory.GenerateResetPasswordEmail(Arg.Any<ResetPasswordEmailProps>())
            .Returns(new EmailModel("fake", "fake"));

        _emailFactory.GenerateInvitationExistingUserEmail(Arg.Any<InvitationExistingUserEmailProps>())
            .Returns(new EmailModel("fake", "fake"));

        _emailFactory.GenerateNewUserInvitationEmail(Arg.Any<InvitationNewUserEmailProps>())
            .Returns(new EmailModel("fake", "fake"));

        _emailFactory.GenerateCitizenReportEmail(Arg.Any<CitizenReportEmailProps>())
            .Returns(new EmailModel("fake", "fake"));

        _jobService = Substitute.For<IJobService>();

        await _database.InitialiseAsync();
        _factory = new CustomWebApplicationFactory(_database.GetConnectionString(), _database.GetConnection(),
            _apiTimeProvider, _emailFactory, _jobService);
    }

    public static string DbConnectionString => _database.GetConnectionString();

    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();
            _emailFactory.ClearReceivedCalls();
            _jobService.ClearReceivedCalls();
        }
        catch (Exception e)
        {
            TestContext.Out.WriteLine(e.Message);
        }
    }

    public static HttpClient CreateClient()
    {
        return _factory.CreateClient();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
