using Refit;

namespace SubmissionsFaker.Clients.Token;

public interface ITokenApi
{
    [Post("/api/auth/login")]
    Task<LoginResponse> GetToken([Body] Credentials credentials);
}