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
using SubmissionsFaker.Consts;
using SubmissionsFaker.Fakers;
using SubmissionsFaker.Forms;
using SubmissionsFaker.Scenarios;
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

var observerApi = RestService.For<IMonitoringObserverApi>(client);
var citizenReportApi = RestService.For<ICitizenApi>(client);

#endregion

List<PollingStationNode> pollingStations = [];
List<LocationNode> locations = [];
List<UpdateFormResponse> forms = [];
List<UpdateFormResponse> citizenReportingForms = [];
List<UpdateFormResponse> incidentReportingForms = [];
List<LoginResponse> observersTokens = [];

var observers = new ApplicationUserFaker().Generate(SeederVars.NUMBER_OF_OBSERVERS)!;

ScenarioData scenarioData = default!;
await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var setupTask = ctx.AddTask("[green]Setup election round and NGO [/]", maxValue: 4);

        scenarioData = ScenarioBuilder.New(() => new HttpClient(new LoggingHandler(new HttpClientHandler()))
            {
                BaseAddress = new Uri("https://localhost:7123"),
            })
            .WithObserver(ScenarioObserver.Alice)
            .WithObserver(ScenarioObserver.Bob)
            .WithNgo(ScenarioNgos.Alfa)
            .WithNgo(ScenarioNgos.Beta)
            .WithElectionRound(ScenarioElectionRound.A, er => er
                .WithPollingStation(ScenarioPollingStation.Iasi)
                .WithPollingStation(ScenarioPollingStation.Bacau)
                .WithPollingStation(ScenarioPollingStation.Cluj)
                .WithMonitoringNgo(ScenarioNgos.Alfa)
                .WithCoalition(ScenarioCoalition.Youth, ScenarioNgos.Alfa, [ScenarioNgos.Beta], cfg => cfg
                    .WithMonitoringObserver(ScenarioNgo.Alfa, ScenarioObserver.Alice)
                    .WithMonitoringObserver(ScenarioNgo.Beta, ScenarioObserver.Bob)
                    .WithForm("Shared", [ScenarioNgos.Alfa, ScenarioNgos.Beta], form => form
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                        .WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Cluj)
                        .WithSubmission(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                    )
                    .WithForm("A", [ScenarioNgos.Alfa],
                        form => form.WithSubmission(ScenarioObserver.Alice, ScenarioPollingStation.Bacau))
                    .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Alice, ScenarioPollingStation.Cluj)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Iasi)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Bacau)
                    .WithQuickReport(ScenarioObserver.Bob, ScenarioPollingStation.Cluj)
                ))
            .Please();


        await scenarioData.PlatformAdminClient.CreatePSIForm(scenarioData.ElectionRoundId, PSIFormData.PSIForm);
        setupTask.Increment(1);

        await scenarioData.PlatformAdminClient.EnableCitizenReporting(scenarioData.ElectionRoundId,
            new EnableCitizenReportingRequest(scenarioData.NgoIdByName(ScenarioNgo.Alfa)));
        setupTask.Increment(1);

        #region import polling stations

        using var pollingStationsStream = File.OpenRead("polling-stations.csv");
        await scenarioData.PlatformAdminClient.ImportPollingStations(scenarioData.ElectionRoundId,
            new StreamPart(pollingStationsStream, "polling-stations.csv", "text/csv"));

        var pollingStationNodes =
            await scenarioData.PlatformAdminClient.GetAllPollingStations(scenarioData.ElectionRoundId);
        pollingStations = faker
            .PickRandom(pollingStationNodes.Nodes.Where(x => x.PollingStationId.HasValue).ToList(),
                SeederVars.NUMBER_OF_POLLING_STATIONS_TO_VISIT)
            .ToList();

        setupTask.Increment(1);

        #endregion

        #region import locations

        using var locationsStream = File.OpenRead("locations.csv");
        await scenarioData.PlatformAdminClient.ImportLocations(scenarioData.ElectionRoundId,
            new StreamPart(locationsStream, "locations.csv", "text/csv"));

        var locationsNodes = await scenarioData.PlatformAdminClient.GetAllLocations(scenarioData.ElectionRoundId);
        locations = faker
            .PickRandom(locationsNodes.Nodes.Where(x => x.LocationId.HasValue).ToList(),
                SeederVars.NUMBER_OF_LOCATIONS_TO_VISIT)
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
        forms = await FormsSeeder.Seed(scenarioData.AdminApiOfNgo(ScenarioNgo.Alfa), scenarioData.ElectionRoundId,
            formsTask);
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
            await CitizenReportingFormSeeder.Seed(scenarioData.AdminApiOfNgo(ScenarioNgo.Alfa),
                scenarioData.ElectionRoundId, formsTask);
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
            await IncidentReportingFormSeeder.Seed(scenarioData.AdminApiOfNgo(ScenarioNgo.Alfa),
                scenarioData.ElectionRoundId, formsTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var observersTask = ctx.AddTask("[green]Seeding observers[/]", maxValue: SeederVars.NUMBER_OF_OBSERVERS,
            autoStart: false);

        await ObserversSeeder.Seed(scenarioData.PlatformAdminClient, observers, scenarioData.ElectionRoundId,
            scenarioData.ElectionRound.MonitoringNgoIdByName(ScenarioNgo.Alfa),
            observersTask);
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var observersLoginTask =
            ctx.AddTask("[green]Logging in observers[/]", maxValue: SeederVars.NUMBER_OF_OBSERVERS);

        foreach (var observersChunk in observers.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = new List<Task<LoginResponse>>();
            foreach (var observer in observersChunk)
            {
                var loginTask = tokenApi.GetToken(new Credentials(observer.Email, observer.Password));
                tasks.Add(loginTask);
            }

            var tokens = await Task.WhenAll(tasks);
            observersTokens.AddRange(tokens);
            observersLoginTask.Increment(SeederVars.CHUNK_SIZE);
        }
    });

var submissionRequests = new SubmissionFaker(forms, pollingStations, observersTokens)
    .GenerateUnique(SeederVars.NUMBER_OF_SUBMISSIONS);

var psiRequests =
    submissionRequests
        .Select(x => new PISSubmissionFaker(PSIFormData.PSIForm, x.PollingStationId, x.ObserverToken).Generate())
        .ToList();

var noteRequests = new NoteFaker(submissionRequests.Where(x => x.Answers.Any()).ToList())
    .Generate(SeederVars.NUMBER_OF_NOTES);

var attachmentRequests = new AttachmentFaker(submissionRequests.Where(x => x.Answers.Any()).ToList())
    .Generate(SeederVars.NUMBER_OF_ATTACHMENTS);

var quickReportRequests = new QuickReportRequestFaker(pollingStations, observersTokens)
    .Generate(SeederVars.NUMBER_OF_QUICK_REPORTS);

var quickReportAttachmentRequests = new QuickReportAttachmentFaker(quickReportRequests)
    .GenerateUnique(SeederVars.NUMBER_OF_QUICK_REPORTS_ATTACHMENTS);

var citizenReportRequests = new CitizenReportsFaker(citizenReportingForms, locations)
    .GenerateUnique(SeederVars.NUMBER_OF_CITIZEN_REPORTS);

var incidentReportRequests = new IncidentReportFaker(incidentReportingForms, pollingStations, observersTokens)
    .GenerateUnique(SeederVars.NUMBER_OF_INCIDENT_REPORTS);

var citizenReportNoteRequests = new CitizenReportNoteFaker(citizenReportRequests.Where(x => x.Answers.Any()).ToList())
    .Generate(SeederVars.NUMBER_OF_CITIZEN_REPORTS_NOTES);

// var citizenReportAttachmentRequests =
//     new CitizenReportAttachmentFaker(citizenReportRequests.Where(x => x.Answers.Any()).ToList())
//         .Generate(SeederVars.NUMBER_OF_CITIZEN_REPORTS_ATTACHMENTS);

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking PSI submissions [/]", maxValue: SeederVars.NUMBER_OF_SUBMISSIONS);
        foreach (var submissionRequestChunk in psiRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = submissionRequestChunk.Select(sr =>
                observerApi.SubmitPSIForm(scenarioData.ElectionRoundId, sr.PollingStationId, sr, sr.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking submissions [/]", maxValue: SeederVars.NUMBER_OF_SUBMISSIONS);
        foreach (var submissionRequestChunk in submissionRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = submissionRequestChunk.Select(sr =>
                observerApi.SubmitForm(scenarioData.ElectionRoundId, sr, sr.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking notes[/]", maxValue: SeederVars.NUMBER_OF_NOTES);

        foreach (var notesChunk in noteRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = notesChunk.Select(n =>
                observerApi.SubmitNote(scenarioData.ElectionRoundId, n, n.ObserverToken));
            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking attachments[/]", maxValue: SeederVars.NUMBER_OF_ATTACHMENTS);

        foreach (var ar in attachmentRequests)
        {
            var fileName = faker.PickRandom(images);
            await using var fileStream = File.OpenRead(Path.Combine("Attachments", fileName));
            await observerApi.SubmitAttachment(scenarioData.ElectionRoundId,
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
        var progressTask = ctx.AddTask("[green]Faking citizen reports [/]",
            maxValue: SeederVars.NUMBER_OF_CITIZEN_REPORTS);
        foreach (var citizenReportBatch in citizenReportRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = citizenReportBatch.Select(sr => citizenReportApi.SubmitForm(scenarioData.ElectionRoundId, sr));

            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking incident reports [/]",
            maxValue: SeederVars.NUMBER_OF_INCIDENT_REPORTS);
        foreach (var incidentReportBatch in incidentReportRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = incidentReportBatch.Select(ir =>
                observerApi.SubmitIncidentReport(scenarioData.ElectionRoundId, ir, ir.ObserverToken));

            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
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
            maxValue: SeederVars.NUMBER_OF_CITIZEN_REPORTS_NOTES);

        foreach (var notesChunk in citizenReportNoteRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = notesChunk.Select(n =>
                citizenReportApi.SubmitNote(scenarioData.ElectionRoundId, n.CitizenReportId, n));
            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
        }
    });

await AnsiConsole.Progress()
    .AutoRefresh(true)
    .AutoClear(false)
    .HideCompleted(false)
    .Columns(progressColumns)
    .StartAsync(async ctx =>
    {
        var progressTask = ctx.AddTask("[green]Faking quick reports[/]", maxValue: SeederVars.NUMBER_OF_QUICK_REPORTS);

        foreach (var quickReportChunk in quickReportRequests.Chunk(SeederVars.CHUNK_SIZE))
        {
            var tasks = quickReportChunk.Select(qr =>
                observerApi.SubmitQuickReport(scenarioData.ElectionRoundId, qr, qr.ObserverToken));
            await Task.WhenAll(tasks);
            progressTask.Increment(SeederVars.CHUNK_SIZE);
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
            maxValue: SeederVars.NUMBER_OF_QUICK_REPORTS_ATTACHMENTS);

        foreach (var qar in quickReportAttachmentRequests)
        {
            var fileName = faker.PickRandom(images);
            await using var fs = File.OpenRead(Path.Combine("Attachments", fileName));
            await observerApi.SubmitQuickReportAttachment(scenarioData.ElectionRoundId, qar.QuickReportId.ToString(),
                qar.Id.ToString(), new StreamPart(fs, fileName, "image/jpeg"), qar.ObserverToken);

            progressTask.Increment(1);
        }
    });

var rule = new Rule("[red]Finished[/]");
AnsiConsole.Write(rule);