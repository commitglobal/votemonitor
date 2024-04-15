using Refit;

namespace SubmissionsFaker.Clients.Token;

public interface ITokenClient
{
    [Post("/api/auth/login")]
    Task<LoginResponse> GetToken([Body] Credentials credentials);
}