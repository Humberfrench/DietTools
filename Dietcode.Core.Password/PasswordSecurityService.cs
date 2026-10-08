using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;
using Dietcode.Core.Password.Scoring;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Password;

public sealed class PasswordSecurityService : IPasswordSecurityService
{
    private readonly IPasswordValidationService _passwordValidationService;
    private readonly PasswordValidationOptions _options;

    public PasswordSecurityService(
        IPasswordValidationService passwordValidationService,
        IOptions<PasswordValidationOptions> options)
    {
        _passwordValidationService = passwordValidationService;
        _options = options.Value;
    }

    public async Task<PasswordSecurityResult> CheckAsync(
        string password,
        PasswordValidationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        var validation = await _passwordValidationService.ValidateAsync(password, context, cancellationToken);

        // Reaproveita MinimumLength de PasswordValidationOptions em vez de um
        // segundo valor de configuração para a mesma coisa.
        var strength = password.AsSpan().AnalyzePassword(_options.MinimumLength);

        return new PasswordSecurityResult
        {
            Validation = validation,
            Strength = strength
        };
    }
}
