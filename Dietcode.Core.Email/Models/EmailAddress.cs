namespace Dietcode.Core.Email.Models;

public sealed class EmailAddress
{
    public EmailAddress()
    {
    }

    public EmailAddress(string email, string? name = null)
    {
        Email = email;
        Name = name;
    }

    public string Email { get; set; } = string.Empty;

    public string? Name { get; set; }

    public static EmailAddress Create(string email, string? name = null)
        => new(email, name);

    public static implicit operator EmailAddress(string email)
        => new(email);
}
