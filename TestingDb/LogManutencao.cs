using Dietcode.Database.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;

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
