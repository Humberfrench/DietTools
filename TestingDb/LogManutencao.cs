using Dietcode.Database.Attribute;

namespace TestingDb
{
    [TableName("LogManutencao")]
    public class LogManutencao
    {
        public LogManutencao()
        {
            LogManutencaoId = 0;
            Processo = string.Empty;
            Comando = string.Empty;
            Data = DateTime.Now;
        }

        [KeyId]
        public int LogManutencaoId { get; set; }
        public string Processo { get; set; }
        public string Comando { get; set; }
        public DateTime Data { get; set; }
    }
}
