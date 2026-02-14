namespace ReproIVF.WebAPI.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "ReproIVF.WebAPI";
    public string Audience { get; set; } = "ReproIVF.Client";
    public int ExpirationMinutes { get; set; } = 120;
}
