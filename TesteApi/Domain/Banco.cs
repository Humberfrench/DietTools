using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TesteApi.Exceptions;

namespace TesteApi.Domain
{
    [Table("Bancos", Schema = "dbo")]
    public partial class Banco
    {
        [Key]
        [Column("BancoId")]
        public int BancoId { get; set; }

        [Column("NomeReduzido")]
        [Required]
        [StringLength(100)]
        public string NomeReduzido { get; set; } = string.Empty;

        [Column("Nome")]
        [Required]
        [StringLength(150)]
        public string Nome { get; set; } = string.Empty;

        [Column("InicioOperacao")]
        [Required]
        public DateTime InicioOperacao { get; set; }

        [Column("CodigoBanco")]
        [Required]
        [StringLength(10)]
        public string CodigoBanco { get; set; } = string.Empty;

        [Column("ISPB")]
        [Required]
        [StringLength(8)]
        public string ISPB { get; set; } = string.Empty;

        [Column("ParticipaCompensacao")]
        [Required]
        public bool ParticipaCompensacao { get; set; }

        [Column("AcessoPrincipal")]
        [Required]
        [StringLength(50)]
        public string AcessoPrincipal { get; set; } = string.Empty;


        public void AtualizarDados(
            string nomeReduzido,
            string nome,
            DateTime inicioOperacao,
            string codigoBanco,
            string ispb,
            bool participaCompensacao,
            string acessoPrincipal)
        {
            NomeReduzido = nomeReduzido;
            Nome = nome;
            InicioOperacao = inicioOperacao;
            CodigoBanco = codigoBanco;
            ISPB = ispb;
            ParticipaCompensacao = participaCompensacao;
            AcessoPrincipal = acessoPrincipal;

            Validar();
        }

        public void Validar()
        {
            if (string.IsNullOrWhiteSpace(NomeReduzido))
                throw new DomainException("Nome reduzido do banco e obrigatorio.");

            if (string.IsNullOrWhiteSpace(Nome))
                throw new DomainException("Nome do banco e obrigatorio.");

            if (InicioOperacao == default)
                throw new DomainException("Inicio de operacao do banco e obrigatorio.");

            if (string.IsNullOrWhiteSpace(CodigoBanco))
                throw new DomainException("Codigo do banco e obrigatorio.");

            if (string.IsNullOrWhiteSpace(ISPB))
                throw new DomainException("ISPB do banco e obrigatorio.");

            if (string.IsNullOrWhiteSpace(AcessoPrincipal))
                throw new DomainException("Acesso principal do banco e obrigatorio.");

            NomeReduzido = NomeReduzido.Trim();
            Nome = Nome.Trim();
            CodigoBanco = CodigoBanco.Trim();
            ISPB = ISPB.Trim();
            AcessoPrincipal = AcessoPrincipal.Trim();

            if (NomeReduzido.Length > 100)
                throw new DomainException("Nome reduzido deve ter no maximo 100 caracteres.");

            if (Nome.Length > 150)
                throw new DomainException("Nome deve ter no maximo 150 caracteres.");

            if (CodigoBanco.Length > 10)
                throw new DomainException("Codigo do banco deve ter no maximo 10 caracteres.");

            if (ISPB.Length > 8)
                throw new DomainException("ISPB deve ter no maximo 8 caracteres.");

            if (AcessoPrincipal.Length > 50)
                throw new DomainException("Acesso principal deve ter no maximo 50 caracteres.");
        }
    }
}
