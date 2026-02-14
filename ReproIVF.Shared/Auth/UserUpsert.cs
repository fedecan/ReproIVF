namespace ReproIVF.Shared.Auth;

public class UserUpsert
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Password { get; set; }
}
