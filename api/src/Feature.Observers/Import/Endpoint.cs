using Feature.Observers.Parser;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Core.Services.Parser;

namespace Feature.Observers.Import;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<ObserverAggregate> repository,
    IRepository<ImportValidationErrors> errorRepo,
    ICsvParser<ObserverImportModel> parser,
    ILogger<Endpoint> logger)
    : Endpoint<Request, Results<NoContent, BadRequest<ImportValidationErrorModel>>>
{
    public override void Configure()
    {
        Post("/api/observers:import");
        DontAutoTag();
        Options(x => x.WithTags("observers"));
        AllowFileUploads();
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, BadRequest<ImportValidationErrorModel>>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var parsingResult = parser.Parse(req.File.OpenReadStream());
        if (parsingResult is ParsingResult<ObserverImportModel>.Fail failedResult)
        {

            string csv = failedResult.Items.ConstructErrorFileContent();
            var errorSaved = await errorRepo.AddAsync(new(ImportType.Observer, req.File.Name, csv), ct);
            return TypedResults.BadRequest(new ImportValidationErrorModel { Id = errorSaved.Id, Message = "The file contains errors! Please use the ID to get the file with the errors described inside." });
        }

        var importedRows = parsingResult as ParsingResult<ObserverImportModel>.Success;

        var applicationUsers = importedRows!
            .Items
            .Select(x => ApplicationUser.CreateObserver(x.FirstName, x.LastName, x.Email, x.PhoneNumber, x.Password))
            .ToDictionary(x => x.NormalizedEmail!);

        var existingUsers = await userManager.Users
            .Where(user => applicationUsers.Keys.Contains(user.NormalizedEmail!))
            .ToListAsync(ct);

        foreach (var applicationUser in applicationUsers)
        {
            var isDuplicatedAccount = existingUsers.Any(x => x.NormalizedEmail == applicationUser.Key);

            if (isDuplicatedAccount)
            {
                logger.LogWarning("An Observer with {email} already exists!", applicationUser.Key);
                continue;
            }

            var result = await userManager.CreateAsync(applicationUser.Value);
            if (result.Succeeded)
            {
                await repository.AddAsync(ObserverAggregate.Create(applicationUser.Value), ct);
            }
            else
            {
                logger.LogError("An error occured when creating user with {email}!", applicationUser.Key);
            }
        }
        
        return TypedResults.NoContent();
    }
}
