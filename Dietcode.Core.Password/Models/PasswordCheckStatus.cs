namespace Dietcode.Core.Password.Models;

public enum PasswordCheckStatus
{
    Safe = 0,
    Compromised = 1,
    ProviderUnavailable = 2,
    InvalidInput = 3,
    Error = 4
}
