// See https://aka.ms/new-console-template for more information

using Bogus;
using Refit;
using Spectre.Console;
using SubmissionsFaker;
using SubmissionsFaker.Clients.Models;
using SubmissionsFaker.Clients.MonitoringObserver;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.NgoAdmin.Models;
using SubmissionsFaker.Clients.PlatformAdmin;
using SubmissionsFaker.Clients.PlatformAdmin.Models;
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

var faker = new Faker();

#region constants
const int NUMBER_OF_POLLING_STATIONS_TO_VISIT = 1000;
const int NUMBER_OF_OBSERVERS = 100;

const int NUMBER_OF_SUBMISSIONS = NUMBER_OF_OBSERVERS * 2;

const int NUMBER_OF_QUICK_REPORTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 50;
const int NUMBER_OF_QUICK_REPORTS_ATTACHMENTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 20;

const int NUMBER_OF_NOTES = (int)(NUMBER_OF_OBSERVERS * 0.2) + 50;
const int NUMBER_OF_ATTACHMENTS = (int)(NUMBER_OF_OBSERVERS * 0.2) + 20;

const string platformAdminUsername = "john.doe@example.com";
const string platformAdminPassword = "password123";

const string ngoAdminUsername = "admin@demo.com";
const string ngoAdminPassword = "string";

string[] images =
[
    "1.jpg",
    "2.jpg",
    "3.jpg",
    "4.jpg",
    "5.jpg",
    "6.jpg",
    "7.jpg",
    "8.jpg",
    "9.jpg"
];

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

CreateResponse electionRound = default!;
CreateResponse ngo = default!;
CreateResponse monitoringNgo = default!;
LoginResponse ngoAdminToken = default!;
List<LocationNode> pollingStations = [];
List<UpdateFormResponse> formIds = [];
List<LoginResponse> observersTokens = [];

var observers = new ApplicationUserFaker().Generate(NUMBER_OF_OBSERVERS)!;

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var setupTask = ctx.AddTask("[green]Setup election round and NGO [/]", maxValue: 7);

        electionRound = await platformAdminApi.CreateElectionRound(new ElectionRoundFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);

        await platformAdminApi.CreatePSIForm(electionRound.Id, PSIFormData.PSIForm, platformAdminToken.Token);
        setupTask.Increment(1);

        ngo = await platformAdminApi.CreateNgo(new NgoFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);

        monitoringNgo = await platformAdminApi.AssignNgoToElectionRound(electionRound.Id, new AssignNgoRequest(ngo.Id), platformAdminToken.Token);
        setupTask.Increment(1);

        var ngoAdmin = new ApplicationUserFaker(ngoAdminUsername, ngoAdminPassword).Generate();
        await platformAdminApi.CreateNgoAdmin(ngoAdmin, ngo.Id, platformAdminToken.Token);
        setupTask.Increment(1);

        ngoAdminToken = await tokenApi.GetToken(new Credentials(ngoAdminUsername, ngoAdminPassword));
        setupTask.Increment(1);

        using var pollingStationsStream = File.OpenRead("polling-stations.csv");
        await platformAdminApi.CreatePollingStations(electionRound.Id, new StreamPart(pollingStationsStream, "polling-stations.csv", "text/csv"), platformAdminToken.Token);

        var pollingStationNodes = await pollingStationsApi.GetAllPollingStations(electionRound.Id, platformAdminToken.Token);
        pollingStations = faker
            .PickRandom(pollingStationNodes.Nodes.Where(x => x.PollingStationId.HasValue).ToList(), NUMBER_OF_POLLING_STATIONS_TO_VISIT)
            .ToList();

        setupTask.Increment(1);
    });


await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var formsTask = ctx.AddTask("[green]Creating forms[/]", maxValue: 1, autoStart: false);
        formIds = await FormsSeeder.Seed(ngoAdminApi, ngoAdminToken, electionRound.Id, formsTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var observersTask = ctx.AddTask("[green]Seeding observers[/]", maxValue: NUMBER_OF_OBSERVERS, autoStart: false);

        await ObserversSeeder.Seed(platformAdminApi, platformAdminToken, observers, electionRound.Id, monitoringNgo.Id,
            observersTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var observersLoginTask = ctx.AddTask("[green]Logging in observers[/]", maxValue: NUMBER_OF_OBSERVERS);

        foreach (var observersChunk in observers.Chunk(2))
        {
            var tasks = new List<Task<LoginResponse>>();
            foreach (var observer in observersChunk)
            {
                var loginTask = tokenApi.GetToken(new Credentials(observer.Email, observer.Password));
                tasks.Add(loginTask);
            }

            var tokens = await Task.WhenAll(tasks);
            observersTokens.AddRange(tokens);
            observersLoginTask.Increment(2);
        }
    });

var submissionRequests = new SubmissionFaker(formIds, pollingStations, observersTokens)
    .GenerateUnique(NUMBER_OF_SUBMISSIONS);

var noteRequests = new NoteFaker(submissionRequests)
    .Generate(NUMBER_OF_NOTES);

var attachmentRequests = new AttachmentFaker(submissionRequests)
    .Generate(NUMBER_OF_ATTACHMENTS);

var quickReportRequests = new QuickReportFaker(pollingStations, observersTokens)
    .Generate(NUMBER_OF_QUICK_REPORTS);

var quickReportAttachmentRequests = new QuickReportAttachmentFaker(quickReportRequests)
    .GenerateUnique(NUMBER_OF_QUICK_REPORTS_ATTACHMENTS);

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking submissions [/]", maxValue: NUMBER_OF_SUBMISSIONS);
        foreach (var submissionRequestChunk in submissionRequests.Chunk(25))
        {
            var tasks = submissionRequestChunk.Select(sr => observerApi.SubmitForm(electionRound.Id, sr, sr.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(25);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking notes[/]", maxValue: NUMBER_OF_NOTES);

        foreach (var notesChunk in noteRequests.Chunk(25))
        {
            var tasks = notesChunk.Select(n => observerApi.SubmitNote(electionRound.Id, n, n.ObserverToken));
            await Task.WhenAll(tasks);
            progressTask.Increment(25);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking attachments[/]", maxValue: NUMBER_OF_ATTACHMENTS);

        foreach (var ar in attachmentRequests)
        {
            var fileName = faker.PickRandom(images);
            await using var fileStream = File.OpenRead(Path.Combine("Attachments", fileName));
            await observerApi.SubmitAttachment(electionRound.Id,
                ar.PollingStationId.ToString(),
                ar.Id.ToString(),
                ar.FormId.ToString(),
                ar.QuestionId.ToString(),
                new StreamPart(fileStream, fileName, "image/jpeg"),
                ar.ObserverToken);

            progressTask.Increment(1);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking quick reports[/]", maxValue: NUMBER_OF_QUICK_REPORTS);

        foreach (var quickReportChunk in quickReportRequests.Chunk(25))
        {
            var tasks = quickReportChunk.Select(qr => observerApi.SubmitQuickReport(electionRound.Id, qr, qr.ObserverToken));
            await Task.WhenAll(tasks);
            progressTask.Increment(25);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking quick report attachments[/]", maxValue: NUMBER_OF_QUICK_REPORTS_ATTACHMENTS);

        foreach (var qar in quickReportAttachmentRequests)
        {
            var fileName = faker.PickRandom(images);
            await using var fs = File.OpenRead(Path.Combine("Attachments", fileName));
            await observerApi.SubmitQuickReportAttachment(electionRound.Id, qar.QuickReportId.ToString(), qar.Id.ToString(), new StreamPart(fs, fileName, "image/jpeg"), qar.ObserverToken);

            progressTask.Increment(1);
        }
    });

var rule = new Rule("[red]Finished[/]");
AnsiConsole.Write(rule);
