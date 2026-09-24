namespace Dietcode.Database.Vault
{
    /// <summary>
    /// Abstração de um provedor de Secrets externo (Bitwarden, e futuramente Azure Key Vault,
    /// AWS Secrets Manager etc.). A resolução de connection string nunca depende diretamente
    /// de um provider concreto — sempre passa por essa interface.
    /// </summary>
    public interface IVaultProvider
    {
        string GetSecretValue(string key);
    }
}
