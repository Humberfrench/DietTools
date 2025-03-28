using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingDb
{
    [Table("LogManutencao")]
    public class LogManutencao
    {
        public LogManutencao()
        {
            LogManutencaoId = 0;
            Processo = string.Empty;
            Comando = string.Empty;
            Data = DateTime.Now;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LogManutencaoId { get; set; }
        public string Processo { get; set; }
        public string Comando { get; set; }
        public DateTime Data { get; set; }
    }
}
