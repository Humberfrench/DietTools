namespace Dietcode.Core.Password;

/// <summary>
/// Decide o que acontece quando o provider de senha comprometida está
/// indisponível. Nunca assumir "seguro" por padrão — a aplicação escolhe
/// explicitamente.
/// </summary>
public enum ProviderFailurePolicy
{
    /// <summary>Indisponibilidade bloqueia a criação/troca da senha.</summary>
    FailClosed,

    /// <summary>Indisponibilidade permite continuar; o status fica registrado em <c>BreachStatus</c>.</summary>
    FailOpen
}
