using Dietcode.Core.Password;
using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dietcode.UnitTests.CorePassword;

public class PasswordValidationServiceTests
{
    private sealed class FakeCompromisedPasswordProvider : ICompromisedPasswordProvider
    {
        private readonly PasswordBreachResult _result;

        public FakeCompromisedPasswordProvider(PasswordBreachResult result) => _result = result;

        public Task<PasswordBreachResult> CheckAsync(string password, CancellationToken cancellationToken = default)
            => Task.FromResult(_result);
    }

    private sealed class ThrowingCompromisedPasswordProvider : ICompromisedPasswordProvider
    {
        public Task<PasswordBreachResult> CheckAsync(string password, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("O provider não deveria ter sido chamado.");
    }

    private static PasswordValidationService CreateService(
        PasswordBreachResult providerResult,
        Action<PasswordValidationOptions>? configure = null)
    {
        var options = new PasswordValidationOptions();
        configure?.Invoke(options);

        return new PasswordValidationService(
            new FakeCompromisedPasswordProvider(providerResult),
            Options.Create(options));
    }

    [Fact]
    public async Task ValidateAsync_SenhaVazia_RetornaPasswordRequired()
    {
        var service = CreateService(PasswordBreachResult.Safe("fake"));

        var result = await service.ValidateAsync("");

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordRequired);
    }

    [Fact]
    public async Task ValidateAsync_SenhaCurta_RetornaPasswordTooShort()
    {
        var service = CreateService(PasswordBreachResult.Safe("fake"), o => o.MinimumLength = 12);

        var result = await service.ValidateAsync("curta1!A");

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordTooShort);
    }

    [Fact]
    public async Task ValidateAsync_SenhaLonga_RetornaPasswordTooLong()
    {
        var service = CreateService(PasswordBreachResult.Safe("fake"), o => o.MaximumLength = 10);

        var result = await service.ValidateAsync(new string('a', 20));

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordTooLong);
    }

    [Fact]
    public async Task ValidateAsync_SenhaContemUsername_RetornaIssue()
    {
        var service = CreateService(PasswordBreachResult.Safe("fake"));

        var result = await service.ValidateAsync(
            "joaosilva2024!",
            new PasswordValidationContext { UserName = "joaosilva" });

        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordContainsUsername);
    }

    [Fact]
    public async Task ValidateAsync_SenhaContemEmail_RetornaIssue()
    {
        var service = CreateService(PasswordBreachResult.Safe("fake"));

        var result = await service.ValidateAsync(
            "maria.silva2024!",
            new PasswordValidationContext { Email = "maria.silva@exemplo.com" });

        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordContainsEmail);
    }

    [Fact]
    public async Task ValidateAsync_SenhaSemContextoSuspeito_NaoGeraIssueDeContexto()
    {
        var service = CreateService(PasswordBreachResult.Safe("fake"));

        var result = await service.ValidateAsync(
            "umaSenhaTotalmenteDiferente987!",
            new PasswordValidationContext { UserName = "joaosilva", Email = "maria.silva@exemplo.com" });

        Assert.DoesNotContain(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordContainsUsername);
        Assert.DoesNotContain(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordContainsEmail);
    }

    [Fact]
    public async Task ValidateAsync_SenhaComprometida_RejectAtivoPorPadrao_Bloqueia()
    {
        var service = CreateService(PasswordBreachResult.Compromised(10, "fake"));

        var result = await service.ValidateAsync("SenhaComprometida123!");

        Assert.False(result.IsValid);
        Assert.True(result.IsCompromised);
        Assert.Equal(10, result.BreachOccurrences);
        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordCompromised);
    }

    [Fact]
    public async Task ValidateAsync_SenhaComprometida_RejectDesativado_NaoBloqueiaMasInformaOcorrencias()
    {
        var service = CreateService(
            PasswordBreachResult.Compromised(10, "fake"),
            o => o.RejectCompromisedPasswords = false);

        var result = await service.ValidateAsync("SenhaComprometida123!");

        Assert.True(result.IsValid);
        Assert.True(result.IsCompromised);
        Assert.Equal(10, result.BreachOccurrences);
        Assert.DoesNotContain(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordCompromised);
    }

    [Fact]
    public async Task ValidateAsync_ProviderIndisponivel_FailClosed_Bloqueia()
    {
        var service = CreateService(
            PasswordBreachResult.Unavailable("fake"),
            o => o.ProviderFailurePolicy = ProviderFailurePolicy.FailClosed);

        var result = await service.ValidateAsync("SenhaQualquer123!");

        Assert.False(result.IsValid);
        Assert.Contains(result.Issues, i => i.Code == PasswordValidationIssueCodes.PasswordProviderUnavailable);
        Assert.Equal(PasswordCheckStatus.ProviderUnavailable, result.BreachStatus);
    }

    [Fact]
    public async Task ValidateAsync_ProviderIndisponivel_FailOpen_NaoBloqueiaMasRegistraStatus()
    {
        var service = CreateService(
            PasswordBreachResult.Unavailable("fake"),
            o => o.ProviderFailurePolicy = ProviderFailurePolicy.FailOpen);

        var result = await service.ValidateAsync("SenhaQualquer123!");

        Assert.True(result.IsValid);
        Assert.Equal(PasswordCheckStatus.ProviderUnavailable, result.BreachStatus);
    }

    [Fact]
    public async Task ValidateAsync_CheckCompromisedDesativado_NaoChamaOProvider()
    {
        var options = Options.Create(new PasswordValidationOptions { CheckCompromisedPasswords = false });
        var service = new PasswordValidationService(new ThrowingCompromisedPasswordProvider(), options);

        var result = await service.ValidateAsync("SenhaQualquer123!");

        Assert.True(result.IsValid);
    }
}
