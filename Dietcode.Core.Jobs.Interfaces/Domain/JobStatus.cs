using System.ComponentModel;

namespace Dietcode.Core.Jobs.Interfaces.Domain
{
    public enum JobStatus
    {
        [Description("Nenhum job encontrado para o id fornecido")]
        NotFound,
        [Description("Aguardando processamento/processando")]
        Processing,
        [Description("Processamento concluído com sucesso")]
        Completed,
        [Description("Processamento concluído com falha")]
        Failed,
        [Description("Status desconhecido")]
        Unknown
    }
}
