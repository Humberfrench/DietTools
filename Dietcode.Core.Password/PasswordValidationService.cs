using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;
using Microsoft.Extensions.Options;

namespace Dietcode.Core.Password;

public sealed class PasswordValidationService : IPasswordValidationService
{
    private readonly ICompromisedPasswordProvider _compromisedPasswordProvider;
    private readonly PasswordValidationOptions _options;

    public PasswordValidationService(
        ICompromisedPasswordProvider compromisedPasswordProvider,
        IOptions<PasswordValidationOptions> options)
    {
        _compromisedPasswordProvider = compromisedPasswordProvider;
        _options = options.Value;
    }

    public async Task<PasswordValidationResult> ValidateAsync(
        string password,
        PasswordValidationContext? context = null,
        CancellationToken cancellationToken = default)
    {
        var issues = new List<PasswordValidationIssue>();

        if (string.IsNullOrEmpty(password))
        {
            issues.Add(new PasswordValidationIssue(PasswordValidationIssueCodes.PasswordRequired, "Senha obrigatória."));

            return new PasswordValidationResult
            {
                IsValid = false,
                IsCompromised = false,
                BreachOccurrences = 0,
                BreachStatus = PasswordCheckStatus.InvalidInput,
                Issues = issues
            };
        }

        if (password.Length < _options.MinimumLength)
        {
            issues.Add(new PasswordValidationIssue(
                PasswordValidationIssueCodes.PasswordTooShort,
                $"A senha deve ter ao menos {_options.MinimumLength} caracteres."));
        }

        if (password.Length > _options.MaximumLength)
        {
            issues.Add(new PasswordValidationIssue(
                PasswordValidationIssueCodes.PasswordTooLong,
                $"A senha deve ter no máximo {_options.MaximumLength} caracteres."));
        }

        CheckContext(password, context, issues);

        var breachResult = await CheckCompromisedAsync(password, cancellationToken);
        ApplyBreachResult(breachResult, issues);

        return new PasswordValidationResult
        {
            IsValid = issues.Count == 0,
            IsCompromised = breachResult.IsCompromised,
            BreachOccurrences = breachResult.Occurrences,
            BreachStatus = breachResult.Status,
            Issues = issues
        };
    }

    private async Task<PasswordBreachResult> CheckCompromisedAsync(string password, CancellationToken cancellationToken)
    {
        if (!_options.CheckCompromisedPasswords)
            return PasswordBreachResult.Safe(provider: "none");

        return await _compromisedPasswordProvider.CheckAsync(password, cancellationToken);
    }

    private void ApplyBreachResult(PasswordBreachResult breachResult, List<PasswordValidationIssue> issues)
    {
        if (breachResult.Status == PasswordCheckStatus.ProviderUnavailable)
        {
            if (_options.ProviderFailurePolicy == ProviderFailurePolicy.FailClosed)
            {
                issues.Add(new PasswordValidationIssue(
                    PasswordValidationIssueCodes.PasswordProviderUnavailable,
                    "Não foi possível verificar se a senha é conhecida como comprometida."));
            }

            // FailOpen: segue sem issue bloqueante; BreachStatus já expõe a indisponibilidade pra quem consome.
            return;
        }

        if (breachResult.IsCompromised && _options.RejectCompromisedPasswords)
        {
            issues.Add(new PasswordValidationIssue(
                PasswordValidationIssueCodes.PasswordCompromised,
                "Esta senha aparece em bases conhecidas de senhas comprometidas. Escolha uma senha diferente e exclusiva para esta conta."));
        }
    }

    private static void CheckContext(string password, PasswordValidationContext? context, List<PasswordValidationIssue> issues)
    {
        if (context is null)
            return;

        if (!string.IsNullOrWhiteSpace(context.UserName) &&
            password.Contains(context.UserName, StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new PasswordValidationIssue(
                PasswordValidationIssueCodes.PasswordContainsUsername,
                "A senha não pode conter o nome de usuário."));
        }

        if (!string.IsNullOrWhiteSpace(context.Email))
        {
            var localPart = context.Email.Split('@', 2)[0];

            var containsEmail =
                password.Contains(context.Email, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(localPart) && password.Contains(localPart, StringComparison.OrdinalIgnoreCase));

            if (containsEmail)
            {
                issues.Add(new PasswordValidationIssue(
                    PasswordValidationIssueCodes.PasswordContainsEmail,
                    "A senha não pode conter o e-mail."));
            }
        }
    }
}
