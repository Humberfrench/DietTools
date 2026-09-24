using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Dietcode.Database.Vault
{
    /// <summary>
    /// Ponto único de resolução da connection string, com suporte a dois modos
    /// ("DatabaseProvider" em "DatabaseConfig" no appsettings.json):
    ///   - Context (padrão/retrocompatível): lê ConnectionStrings:{KeyContext ?? "DbContextConnString"}.
    ///   - Vault: busca o valor num provedor externo de Secrets (hoje só Bitwarden).
    /// Resultado é cacheado em processo (Lazy) — evita bater no Vault a cada novo DbContext.
    /// </summary>
    public static class ConnectionStringResolver
    {
        private const string DefaultKeyContext = "DbContextConnString";

        private static readonly Lazy<string> Cached = new(Resolve, isThreadSafe: true);

        public static string Get() => Cached.Value;

        private static string Resolve()
        {
            var configuration = BuildConfiguration();
            var options = configuration.GetSection("DatabaseConfig").Get<DatabaseConfigOptions>();

            if (options is null ||
                string.IsNullOrWhiteSpace(options.DatabaseProvider) ||
                string.Equals(options.DatabaseProvider, "Context", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveFromContext(configuration, options?.KeyContext);
            }

            if (string.Equals(options.DatabaseProvider, "Vault", StringComparison.OrdinalIgnoreCase))
                return ResolveFromVault(configuration, options);

            throw new NotSupportedException(
                $"DatabaseProvider '{options.DatabaseProvider}' não é suportado. Valores válidos: Context, Vault.");
        }

        private static string ResolveFromContext(IConfiguration configuration, string? keyContext)
        {
            var key = string.IsNullOrWhiteSpace(keyContext) ? DefaultKeyContext : keyContext;

            return configuration.GetConnectionString(key)
                ?? throw new ArgumentException($"Connection String Inválida (chave '{key}' não encontrada em ConnectionStrings).");
        }

        private static string ResolveFromVault(IConfiguration configuration, DatabaseConfigOptions options)
        {
            if (!string.Equals(options.VaultProvider, "Bitwarden", StringComparison.OrdinalIgnoreCase))
                throw new NotSupportedException(
                    $"VaultProvider '{options.VaultProvider}' não é suportado. Hoje só 'Bitwarden' está implementado.");

            if (string.IsNullOrWhiteSpace(options.OrganizationId) || !Guid.TryParse(options.OrganizationId, out var organizationId))
                throw new InvalidOperationException("DatabaseConfig.OrganizationId ausente ou inválido para DatabaseProvider = Vault.");

            var key = options.VaultConn?.Key;
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("DatabaseConfig.VaultConn.Key ausente para DatabaseProvider = Vault.");

            var accessToken = configuration["BitwardenAccessToken"] ?? configuration["BWS_ACCESS_TOKEN"];
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new InvalidOperationException(
                    "Access Token do Bitwarden não encontrado. Configure a variável de ambiente BWS_ACCESS_TOKEN " +
                    "(ou a chave 'BitwardenAccessToken' via dotnet user-secrets) — nunca no appsettings.json.");

            IVaultProvider provider = new BitwardenVaultProvider(accessToken, organizationId);
            return provider.GetSecretValue(key);
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is not null)
            {
                try
                {
                    builder.AddUserSecrets(entryAssembly, optional: true);
                }
                catch (InvalidOperationException)
                {
                    // Assembly de entrada sem UserSecretsIdAttribute — segue sem user-secrets.
                }
            }

            return builder.Build();
        }
    }
}
