using Dietcode.Core.Password;
using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;
using Dietcode.Core.Password.Scoring;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dietcode.UnitTests.CorePassword;

public class PasswordSecurityServiceTests
{
    private sealed class FakePasswordValidationService : IPasswordValidationService
    {
        private readonly PasswordValidationResult _result;

        public FakePasswordValidationService(PasswordValidationResult result) => _result = result;

        public Task<PasswordValidationResult> ValidateAsync(
            string password,
            PasswordValidationContext? context = null,
            CancellationToken cancellationToken = default)
            => Task.FromResult(_result);
    }

    private static PasswordValidationResult SafeResult() => new()
    {
        IsValid = true,
        IsCompromised = false,
        BreachOccurrences = 0,
        BreachStatus = PasswordCheckStatus.Safe,
        Issues = Array.Empty<PasswordValidationIssue>()
    };

    private static PasswordValidationResult CompromisedResult() => new()
    {
        IsValid = false,
        IsCompromised = true,
        BreachOccurrences = 10,
        BreachStatus = PasswordCheckStatus.Compromised,
        Issues = new[]
        {
            new PasswordValidationIssue(PasswordValidationIssueCodes.PasswordCompromised, "comprometida")
        }
    };

    private static PasswordSecurityService CreateService(
        PasswordValidationResult validationResult,
        Action<PasswordValidationOptions>? configure = null)
    {
        var options = new PasswordValidationOptions();
        configure?.Invoke(options);

        return new PasswordSecurityService(
            new FakePasswordValidationService(validationResult),
            Options.Create(options));
    }

    [Fact]
    public async Task CheckAsync_CombinaValidacaoEForca()
    {
        var service = CreateService(SafeResult());

        var result = await service.CheckAsync("Senha@Forte123!");

        Assert.True(result.IsValid);
        Assert.False(result.Validation.IsCompromised);
        Assert.True(result.Strength.HasUppercase);
        Assert.True(result.Strength.HasDigit);
        Assert.True(result.Strength.HasSymbol);
        Assert.NotEqual(PasswordStrengthLevel.Invalid, result.Strength.Level);
    }

    [Fact]
    public async Task CheckAsync_SenhaComprometida_IsValidReflexteApenasValidacao()
    {
        var service = CreateService(CompromisedResult());

        var result = await service.CheckAsync("123456");

        Assert.False(result.IsValid);
        Assert.True(result.Validation.IsCompromised);
        // Mesmo invalidada pelo leak-check, a força continua sendo calculada.
        Assert.Equal(PasswordStrengthLevel.VeryWeak, result.Strength.Level);
    }

    [Fact]
    public async Task CheckAsync_UsaMinimumLengthDasOpcoesParaCalcularForca()
    {
        var service = CreateService(SafeResult(), o => o.MinimumLength = 20);

        var result = await service.CheckAsync("Curta@123");

        Assert.False(result.Strength.MeetsMinimumRules);
    }
}
