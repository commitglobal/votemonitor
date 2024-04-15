
namespace Vote.Monitor.Api.Feature.Auth.NgoAdminsOnly;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId).NotEmpty();
    }
}
