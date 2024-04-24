// See https://aka.ms/new-console-template for more information

using Bogus;
using Refit;
using Spectre.Console;
using SubmissionsFaker.Clients;
using SubmissionsFaker.Clients.MonitoringObserver;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.PlatformAdmin;
using SubmissionsFaker.Clients.PollingStations;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Fakers;
using SubmissionsFaker.Forms;
using SubmissionsFaker.Seeders;
using Credentials = SubmissionsFaker.Clients.Token.Credentials;

ProgressColumn[] progressColumns = [
    new TaskDescriptionColumn(),    // Task description
    new ProgressBarColumn(),        // Progress bar
    new PercentageColumn(),         // Percentage
    new SpinnerColumn() // Spinner
];

AnsiConsole.Write(
    new FigletText("Submissions Faker")
        .Centered()
        .Color(Color.Yellow));

#region constants
const int NUMBER_OF_OBSERVERS = 100;
const int NUMBER_OF_SUBMISSIONS = 100;
const string platformAdminUsername = "john.doe@example.com";
const string platformAdminPassword = "password123";

const string ngoAdminUsername = "admin@demo.com";
const string ngoAdminPassword = "string";
#endregion


#region setup clients
var client = new HttpClient
{
    BaseAddress = new Uri("https://localhost:7123")
};

var tokenApi = RestService.For<ITokenApi>(client);
var platformAdminApi = RestService.For<IPlatformAdminApi>(client);
var pollingStationsApi = RestService.For<IPollingStationsApi>(client);
var ngoAdminApi = RestService.For<INgoAdminApi>(client);
var observerApi = RestService.For<IMonitoringObserverApi>(client);
#endregion

#region authorize platform admin
var platformAdminToken = await tokenApi.GetToken(new Credentials(platformAdminUsername, platformAdminPassword));
#endregion

CreateResponse electionRound = new CreateResponse(){Id = Guid.Parse("f331a1e8-b888-4d7b-bb5f-a1e64ff43210") };
CreateResponse ngo = new CreateResponse(){ Id = Guid.Parse("85fc1300-7046-43bb-9814-7a4fd67bec96 ") };
CreateResponse monitoringNgo = default!;
LoginResponse ngoAdminToken = default!;
List<LocationNode> pollingStations = [];
List<CreateResponse> formIds = [];
List<LoginResponse> observersTokens = [];

var observers = new ApplicationUserFaker().Generate(NUMBER_OF_OBSERVERS)!;

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var setupTask = ctx.AddTask("[green]Setup election round and NGO [/]", maxValue: 6);

        // electionRound = await platformAdminApi.CreateElectionRound(new ElectionRoundFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);

            //ngo = await platformAdminApi.CreateNgo(new NgoFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);

        monitoringNgo = await platformAdminApi.AssignNgoToElectionRound(electionRound.Id, new AssignNgoRequest(ngo.Id), platformAdminToken.Token);
        setupTask.Increment(1);

        var ngoAdmin = new ApplicationUserFaker(ngoAdminUsername, ngoAdminPassword).Generate();
        await platformAdminApi.CreateNgoAdmin(ngoAdmin, ngo.Id, platformAdminToken.Token);
        setupTask.Increment(1);

        ngoAdminToken = await tokenApi.GetToken(new Credentials(ngoAdmin.Email, ngoAdmin.Password));
        setupTask.Increment(1);

        using var pollingStationsStream = File.OpenRead("polling-stations.csv");
        await platformAdminApi.CreatePollingStations(electionRound.Id, new StreamPart(pollingStationsStream, "polling-stations.csv", "text/csv"), platformAdminToken.Token);
        var pollingStationNodes = await pollingStationsApi.GetAllPollingStations(electionRound.Id, platformAdminToken.Token);
        pollingStations = pollingStationNodes.Nodes.Where(x => x.PollingStationId.HasValue).ToList();
        setupTask.Increment(1);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var formsTask = ctx.AddTask("[green]Creating forms[/]", maxValue: 3, autoStart: false);
        var observersTask = ctx.AddTask("[green]Seeding observers[/]", maxValue: 2 * NUMBER_OF_OBSERVERS, autoStart: false);

        var ids = await Task.WhenAll([
            FormsSeeder.Seed(ngoAdminApi, ngoAdminToken, electionRound.Id, monitoringNgo.Id, formsTask),
            ObserversSeeder.Seed(platformAdminApi, platformAdminToken, observers, electionRound.Id, monitoringNgo.Id, observersTask)
         ]);

        formIds = ids[0];
    });



await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var observersLoginTask = ctx.AddTask("[green]Logging in observers[/]", maxValue: NUMBER_OF_SUBMISSIONS);

        #region login observers
        foreach (var observersChunk in observers.Chunk(25))
        {
            var tasks = new List<Task<LoginResponse>>();
            foreach (var observer in observersChunk)
            {
                var loginTask = tokenApi.GetToken(new Credentials(observer.Email, observer.Password));
                tasks.Add(loginTask);
            }

            var tokens = await Task.WhenAll(tasks);
            observersTokens.AddRange(tokens);
            observersLoginTask.Increment(25);
        }
        #endregion
    });


await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking submissions[/]", maxValue: NUMBER_OF_SUBMISSIONS);
        var faker = new Faker();
        var submissionRequests = new SubmissionFaker(formIds, pollingStations, FormData.OpeningForm.Questions).Generate(NUMBER_OF_SUBMISSIONS);

        foreach (var submissionsChunk in submissionRequests.Chunk(2))
        {
            var observer = faker.PickRandom(observersTokens)!;
            var tasks = submissionsChunk.Select(submissionRequest => observerApi.SubmitForm(electionRound.Id, submissionRequest, observer.Token)).ToList();
            await Task.WhenAll(tasks);
            progressTask.Increment(2);
        }
    });

var rule = new Rule("[red]Finished[/]");
AnsiConsole.Write(rule);
