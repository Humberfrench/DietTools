namespace Dietcode.Core.Password;

/// <summary>
/// Provider de verificação de senha comprometida, escolhido em
/// <c>AddDietcodePassword(...)</c>. V1 só tem HIBP; o enum já existe pensando
/// num futuro provider Offline (corpus local), igual ao padrão de
/// Dietcode.Core.Cep/Dietcode.Core.Jobs — sem contingência automática por
/// enquanto, só um provider ativo por processo.
/// </summary>
public enum PasswordProviderPrimario
{
    Hibp = 1
}
