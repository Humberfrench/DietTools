using Bitwarden.Sdk;

namespace Dietcode.Database.Vault
{
    public sealed class BitwardenVaultProvider : IVaultProvider
    {
        private readonly string _accessToken;
        private readonly Guid _organizationId;

        public BitwardenVaultProvider(string accessToken, Guid organizationId)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new InvalidOperationException("Access Token do Bitwarden não configurado.");

            _accessToken = accessToken;
            _organizationId = organizationId;
        }

        public string GetSecretValue(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Key do secret não informada.", nameof(key));

            using var client = new BitwardenClient();
            client.Auth.LoginAccessToken(_accessToken);

            var identifiers = client.Secrets.List(_organizationId).Data;
            var match = identifiers.FirstOrDefault(s => string.Equals(s.Key, key, StringComparison.Ordinal))
                ?? throw new InvalidOperationException(
                    $"Secret com key '{key}' não encontrado no Bitwarden (OrganizationId={_organizationId}).");

            var secret = client.Secrets.Get(match.Id);

            return string.IsNullOrWhiteSpace(secret.Value)
                ? throw new InvalidOperationException($"Secret '{key}' encontrado no Bitwarden, mas o valor veio vazio.")
                : secret.Value;
        }
    }
}
