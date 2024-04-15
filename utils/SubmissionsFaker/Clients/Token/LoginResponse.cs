namespace SubmissionsFaker.Clients.Token;

public class LoginResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string[] Roles { get; set; }
}