namespace Dietcode.Core.Jobs;

/// <summary>
/// Provider de infraestrutura (IJobQueue + IAsyncJobStoreGeneric) escolhido em
/// AddDietcodeJobs(...). Sem contingência entre eles (diferente de Dietcode.Core.Cep):
/// fila e store são stateful, então trocar de provider em runtime perderia jobs em voo
/// ou já persistidos no provider anterior. A escolha é explícita e fixa por processo.
/// </summary>
public enum JobsProviderPrimario
{
    InMemory = 1,
    Redis = 2
}
