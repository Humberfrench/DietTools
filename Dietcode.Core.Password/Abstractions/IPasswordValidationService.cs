using Dietcode.Core.Password.Models;

namespace Dietcode.Core.Password.Abstractions;

public interface IPasswordValidationService
{
    Task<PasswordValidationResult> ValidateAsync(
        string password,
        PasswordValidationContext? context = null,
        CancellationToken cancellationToken = default);
}
