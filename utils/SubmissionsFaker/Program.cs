// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Bogus;
using Figgle;
using Refit;
using SubmissionsFaker.Clients;
using SubmissionsFaker.Clients.MonitoringObserver;
using SubmissionsFaker.Clients.MonitoringObserver.Models;
using SubmissionsFaker.Clients.NgoAdmin;
using SubmissionsFaker.Clients.PlatformAdmin;
using SubmissionsFaker.Clients.PollingStations;
using SubmissionsFaker.Clients.Token;
using SubmissionsFaker.Fakers;
using SubmissionsFaker.Forms;
using Credentials = SubmissionsFaker.Clients.Token.Credentials;


Console.WriteLine(FiggleFonts.Standard.Render("Submissions Faker"));

#region setup clients

var client = new HttpClient
{
    BaseAddress = new Uri("https://localhost:7123")
};

var tokenApi = RestService.For<ITokenClient>(client);
var platformAdminApi = RestService.For<IPlatformAdminApi>(client);
var pollingStationsApi = RestService.For<IPollingStationsApi>(client);
var ngoAdminApi = RestService.For<INgoAdminApi>(client);
IMonitoringObserverApi observerApi = RestService.For<IMonitoringObserverApi>(client);

#endregion

#region authorize platform admin

var platformAdminUsername = "john.doe@example.com";
var platformAdminPassword = "password123";

var platformAdminToken = await tokenApi.GetToken(new Credentials(platformAdminUsername, platformAdminPassword));

#endregion

#region create election round
var electionRound = await platformAdminApi.CreateElectionRound(new ElectionRoundFaker().Generate(), platformAdminToken.Token);
#endregion

#region create NGO
var ngo = await platformAdminApi.CreateNgo(new NgoFaker().Generate(), platformAdminToken.Token);
var monitoringNgo = await platformAdminApi.AssignNgoToElectionRound(electionRound.Id, new AssignNgoRequest(ngo.Id), platformAdminToken.Token);
var ngoAdmin = new ApplicationUserFaker().Generate();
await platformAdminApi.CreateNgoAdmin(ngoAdmin, ngo.Id, platformAdminToken.Token);
var ngoAdminToken = await tokenApi.GetToken(new Credentials(ngoAdmin.Email, ngoAdmin.Password));
#endregion

#region create polling stations
using var pollingStationsStream = File.OpenRead("polling-stations.csv");
await platformAdminApi.CreatePollingStations(electionRound.Id, new StreamPart(pollingStationsStream, "polling-stations.csv", "text/csv"), platformAdminToken.Token);
var pollingStationNodes = await pollingStationsApi.GetAllPollingStations(electionRound.Id, platformAdminToken.Token);
var pollingStations = pollingStationNodes.Nodes.Where(x => x.PollingStationId.HasValue).ToList();
#endregion

#region create forms

var openingForm = await ngoAdminApi.CreateForm(electionRound.Id, monitoringNgo.Id, FormData.OpeningForm, ngoAdminToken.Token);
await ngoAdminApi.UpdateForm(electionRound.Id, monitoringNgo.Id, openingForm.Id, FormData.OpeningForm, ngoAdminToken.Token);
#endregion

var observers = new ApplicationUserFaker().Generate(100);
var observerIds = new List<CreateResponse>();

#region create 1000 observers
foreach (var observersChunk in observers.Chunk(25))
{
    var tasks = new List<Task<CreateResponse>>();
    foreach (var observer in observersChunk)
    {
        var createTask = platformAdminApi.CreateObserver(observer, platformAdminToken.Token);
        tasks.Add(createTask);
    }

    var observersChunkIds = await Task.WhenAll(tasks);
    observerIds.AddRange(observersChunkIds);
}

#endregion


#region assign observers to election round
foreach (var observersIdsChunk in observerIds.Chunk(25))
{
    var tasks = new List<Task<CreateResponse>>();
    foreach (var observer in observersIdsChunk)
    {
        var assignTask = platformAdminApi.AssignObserverToMonitoring(electionRound.Id, monitoringNgo.Id, new AssignObserverRequest(observer.Id), platformAdminToken.Token);
        tasks.Add(assignTask);
    }

    await Task.WhenAll(tasks);
}
#endregion
var observersTokens = new List<LoginResponse>();

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
}
#endregion

#region generate submissions

var faker = new Faker();

for (int i = 0; i < 1000; i++)
{
    var pollingStation = faker.PickRandom(pollingStations)!;
    var observer = faker.PickRandom(observersTokens)!;
    var submissionRequest = new SubmissionFaker(openingForm.Id, pollingStation.PollingStationId.Value, FormData.OpeningForm.Questions);

    await observerApi.SubmitForm(electionRound.Id, submissionRequest, observer.Token);
}

#endregion