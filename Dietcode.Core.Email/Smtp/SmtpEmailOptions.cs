namespace Dietcode.Core.Email.Smtp;

public sealed class SmtpEmailOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; } = 587;

    public bool UseSsl { get; set; }

    public bool UseStartTls { get; set; } = true;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string FromEmail { get; set; } = string.Empty;

    public string FromName { get; set; } = string.Empty;

    public int TimeoutSeconds { get; set; } = 120;

    internal IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Host))
            errors.Add("SMTP Host obrigatorio.");

        if (Port <= 0)
            errors.Add("SMTP Port deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(FromEmail))
            errors.Add("SMTP FromEmail obrigatorio.");

        if (TimeoutSeconds <= 0)
            errors.Add("SMTP TimeoutSeconds deve ser maior que zero.");

        return errors;
    }
}
