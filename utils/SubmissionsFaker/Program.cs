using Bogus;
using Refit;
using Spectre.Console;
using SubmissionsFaker;
using SubmissionsFaker.Clients.Citizen;
using SubmissionsFaker.Clients.Locations;
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

ProgressColumn[] progressColumns =
[
    new TaskDescriptionColumn(), // Task description
    new ProgressBarColumn(), // Progress bar
    new PercentageColumn(), // Percentage
    new SpinnerColumn() // Spinner
];

AnsiConsole.Write(
    new FigletText("Submissions Faker")
        .Centered()
        .Color(Color.Yellow));

var faker = new Faker();

#region constants

const string platformAdminUsername = "john.doe@example.com";
const string platformAdminPassword = "password123";

const string ngoAdminUsername = "fake@admin.com";
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

var client = new HttpClient(new LoggingHandler(new HttpClientHandler()))
{
    BaseAddress = new Uri("https://localhost:7123"),
    
};

var tokenApi = RestService.For<ITokenApi>(client);
var platformAdminApi = RestService.For<IPlatformAdminApi>(client);
var pollingStationsApi = RestService.For<IPollingStationsApi>(client);
var locationsApi = RestService.For<ILocationsApi>(client);
var ngoAdminApi = RestService.For<INgoAdminApi>(client);
var observerApi = RestService.For<IMonitoringObserverApi>(client);
var citizenReportApi = RestService.For<ICitizenApi>(client);

#endregion

#region authorize platform admin

var platformAdminToken = await tokenApi.GetToken(new Credentials(platformAdminUsername, platformAdminPassword));

#endregion

CreateResponse electionRound = default!;
CreateResponse electionRound2 = default!;
CreateResponse ngo = default!;
CreateResponse monitoringNgo = default!;
LoginResponse ngoAdminToken = default!;
List<PollingStationNode> pollingStations = [];
List<LocationNode> locations = [];
List<UpdateFormResponse> forms = [];
List<UpdateFormResponse> citizenReportingForms = [];
List<UpdateFormResponse> incidentReportingForms = [];
List<LoginResponse> observersTokens = [];

var observers = new ApplicationUserFaker().Generate(Consts.NUMBER_OF_OBSERVERS)!;

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var setupTask = ctx.AddTask("[green]Setup election round and NGO [/]", maxValue: 11);

        electionRound =
            await platformAdminApi.CreateElectionRound(new ElectionRoundFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);
        
        electionRound2 =
            await platformAdminApi.CreateElectionRound(new ElectionRoundFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);

        await platformAdminApi.CreatePSIForm(electionRound.Id, PSIFormData.PSIForm, platformAdminToken.Token);
        setupTask.Increment(1);

        ngo = await platformAdminApi.CreateNgo(new NgoFaker().Generate(), platformAdminToken.Token);
        setupTask.Increment(1);

        monitoringNgo = await platformAdminApi.AssignNgoToElectionRound(electionRound.Id, new AssignNgoRequest(ngo.Id),
            platformAdminToken.Token);
        setupTask.Increment(1);
        
        await platformAdminApi.AssignNgoToElectionRound(electionRound2.Id, new AssignNgoRequest(ngo.Id),
            platformAdminToken.Token);
        setupTask.Increment(1);

        await platformAdminApi.EnableCitizenReporting(electionRound.Id, new EnableCitizenReportingRequest(ngo.Id),
            platformAdminToken.Token);
        setupTask.Increment(1);

        var ngoAdmin = new ApplicationUserFaker(ngoAdminUsername, ngoAdminPassword).Generate();
        await platformAdminApi.CreateNgoAdmin(ngoAdmin, ngo.Id, platformAdminToken.Token);
        setupTask.Increment(1);

        ngoAdminToken = await tokenApi.GetToken(new Credentials(ngoAdminUsername, ngoAdminPassword));
        setupTask.Increment(1);

        #region import polling stations

        using var pollingStationsStream = File.OpenRead("polling-stations.csv");
        await platformAdminApi.ImportPollingStations(electionRound.Id,
            new StreamPart(pollingStationsStream, "polling-stations.csv", "text/csv"), platformAdminToken.Token);

        var pollingStationNodes =
            await pollingStationsApi.GetAllPollingStations(electionRound.Id, platformAdminToken.Token);
        pollingStations = faker
            .PickRandom(pollingStationNodes.Nodes.Where(x => x.PollingStationId.HasValue).ToList(),
                Consts.NUMBER_OF_POLLING_STATIONS_TO_VISIT)
            .ToList();

        setupTask.Increment(1);

        #endregion

        #region import locations

        using var locationsStream = File.OpenRead("locations.csv");
        await platformAdminApi.ImportLocations(electionRound.Id,
            new StreamPart(locationsStream, "locations.csv", "text/csv"), platformAdminToken.Token);

        var locationsNodes = await locationsApi.GetAllLocations(electionRound.Id, platformAdminToken.Token);
        locations = faker
            .PickRandom(locationsNodes.Nodes.Where(x => x.LocationId.HasValue).ToList(),
                Consts.NUMBER_OF_LOCATIONS_TO_VISIT)
            .ToList();

        setupTask.Increment(1);

        #endregion
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var formsTask = ctx.AddTask("[green]Creating forms[/]", autoStart: false);
        forms = await FormsSeeder.Seed(ngoAdminApi, ngoAdminToken, electionRound.Id, formsTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var formsTask = ctx.AddTask("[green]Creating citizen reporting forms[/]", autoStart: false);
        citizenReportingForms =
            await CitizenReportingFormSeeder.Seed(ngoAdminApi, ngoAdminToken, electionRound.Id, formsTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var formsTask = ctx.AddTask("[green]Creating incident reporting forms[/]", autoStart: false);
        incidentReportingForms =
            await IncidentReportingFormSeeder.Seed(ngoAdminApi, ngoAdminToken, electionRound.Id, formsTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var observersTask = ctx.AddTask("[green]Seeding observers[/]", maxValue: Consts.NUMBER_OF_OBSERVERS,
            autoStart: false);

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
        var observersLoginTask = ctx.AddTask("[green]Logging in observers[/]", maxValue: Consts.NUMBER_OF_OBSERVERS);

        foreach (var observersChunk in observers.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = new List<Task<LoginResponse>>();
            foreach (var observer in observersChunk)
            {
                var loginTask = tokenApi.GetToken(new Credentials(observer.Email, observer.Password));
                tasks.Add(loginTask);
            }

            var tokens = await Task.WhenAll(tasks);
            observersTokens.AddRange(tokens);
            observersLoginTask.Increment(Consts.CHUNK_SIZE);
        }
    });

var submissionRequests = new SubmissionFaker(forms, pollingStations, observersTokens)
    .GenerateUnique(Consts.NUMBER_OF_SUBMISSIONS);

var psiRequests =
    submissionRequests
        .Select(x => new PISSubmissionFaker(PSIFormData.PSIForm, x.PollingStationId, x.ObserverToken).Generate())
        .ToList();

var noteRequests = new NoteFaker(submissionRequests.Where(x => x.Answers.Any()).ToList())
    .Generate(Consts.NUMBER_OF_NOTES);

var attachmentRequests = new AttachmentFaker(submissionRequests.Where(x => x.Answers.Any()).ToList())
    .Generate(Consts.NUMBER_OF_ATTACHMENTS);

var quickReportRequests = new QuickReportFaker(pollingStations, observersTokens)
    .Generate(Consts.NUMBER_OF_QUICK_REPORTS);

var quickReportAttachmentRequests = new QuickReportAttachmentFaker(quickReportRequests)
    .GenerateUnique(Consts.NUMBER_OF_QUICK_REPORTS_ATTACHMENTS);

var citizenReportRequests = new CitizenReportsFaker(citizenReportingForms, locations)
    .GenerateUnique(Consts.NUMBER_OF_CITIZEN_REPORTS);

var incidentReportRequests = new IncidentReportFaker(incidentReportingForms, pollingStations, observersTokens)
    .GenerateUnique(Consts.NUMBER_OF_INCIDENT_REPORTS);

var citizenReportNoteRequests = new CitizenReportNoteFaker(citizenReportRequests.Where(x => x.Answers.Any()).ToList())
    .Generate(Consts.NUMBER_OF_CITIZEN_REPORTS_NOTES);

var citizenReportAttachmentRequests =
    new CitizenReportAttachmentFaker(citizenReportRequests.Where(x => x.Answers.Any()).ToList())
        .Generate(Consts.NUMBER_OF_CITIZEN_REPORTS_ATTACHMENTS);

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking PSI submissions [/]", maxValue: Consts.NUMBER_OF_SUBMISSIONS);
        foreach (var submissionRequestChunk in psiRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = submissionRequestChunk.Select(sr =>
                observerApi.SubmitPSIForm(electionRound.Id, sr.PollingStationId, sr, sr.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking submissions [/]", maxValue: Consts.NUMBER_OF_SUBMISSIONS);
        foreach (var submissionRequestChunk in submissionRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = submissionRequestChunk.Select(sr =>
                observerApi.SubmitForm(electionRound.Id, sr, sr.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking notes[/]", maxValue: Consts.NUMBER_OF_NOTES);

        foreach (var notesChunk in noteRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = notesChunk.Select(n => observerApi.SubmitNote(electionRound.Id, n, n.ObserverToken));
            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking attachments[/]", maxValue: Consts.NUMBER_OF_ATTACHMENTS);

        foreach (var ar in attachmentRequests)
        {
            var fileName = faker.PickRandom(images);
            await using var fileStream = File.OpenRead(Path.Combine("Attachments", fileName));
            await observerApi.SubmitAttachment(electionRound.Id,
                ar.PollingStationId.ToString(),
                ar.Id.ToString(),
                ar.FormId,
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
        var progressTask = ctx.AddTask("[green]Faking citizen reports [/]", maxValue: Consts.NUMBER_OF_CITIZEN_REPORTS);
        foreach (var citizenReportBatch in citizenReportRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = citizenReportBatch.Select(sr => citizenReportApi.SubmitForm(electionRound.Id, sr));

            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking incident reports [/]", maxValue: Consts.NUMBER_OF_INCIDENT_REPORTS);
        foreach (var incidentReportBatch in incidentReportRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = incidentReportBatch.Select(ir =>
                observerApi.SubmitIncidentReport(electionRound.Id, ir, ir.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking citizen reports notes[/]",
            maxValue: Consts.NUMBER_OF_CITIZEN_REPORTS_NOTES);

        foreach (var notesChunk in citizenReportNoteRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = notesChunk.Select(n => citizenReportApi.SubmitNote(electionRound.Id, n.CitizenReportId, n));
            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking quick reports[/]", maxValue: Consts.NUMBER_OF_QUICK_REPORTS);

        foreach (var quickReportChunk in quickReportRequests.Chunk(Consts.CHUNK_SIZE))
        {
            var tasks = quickReportChunk.Select(qr =>
                observerApi.SubmitQuickReport(electionRound.Id, qr, qr.ObserverToken));
            await Task.WhenAll(tasks);
            progressTask.Increment(Consts.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking quick report attachments[/]",
            maxValue: Consts.NUMBER_OF_QUICK_REPORTS_ATTACHMENTS);

        foreach (var qar in quickReportAttachmentRequests)
        {
            var fileName = faker.PickRandom(images);
            await using var fs = File.OpenRead(Path.Combine("Attachments", fileName));
            await observerApi.SubmitQuickReportAttachment(electionRound.Id, qar.QuickReportId.ToString(),
                qar.Id.ToString(), new StreamPart(fs, fileName, "image/jpeg"), qar.ObserverToken);

            progressTask.Increment(1);
        }
    });

var rule = new Rule("[red]Finished[/]");
AnsiConsole.Write(rule);