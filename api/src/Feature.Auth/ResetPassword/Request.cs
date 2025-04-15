namespace Feature.Auth.ResetPassword;

public class Request
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Token { get; set; } = null!;
}
