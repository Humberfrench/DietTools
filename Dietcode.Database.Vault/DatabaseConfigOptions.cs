namespace Dietcode.Database.Vault
{
    public sealed class DatabaseConfigOptions
    {
        public string? DatabaseProvider { get; set; }
        public string? VaultProvider { get; set; }
        public string? KeyContext { get; set; }
        public string? OrganizationId { get; set; }
        public VaultConnOptions? VaultConn { get; set; }
    }

    public sealed class VaultConnOptions
    {
        public string? Key { get; set; }
    }
}
