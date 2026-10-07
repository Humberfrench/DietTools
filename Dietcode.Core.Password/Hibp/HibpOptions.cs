namespace Dietcode.Core.Password.Hibp;

public sealed class HibpOptions
{
    public string BaseUrl { get; set; } = "https://api.pwnedpasswords.com/";

    public bool Enabled { get; set; } = true;

    public int TimeoutSeconds { get; set; } = 5;

    public bool UsePadding { get; set; } = true;

    public int CacheMinutes { get; set; } = 60;
}
