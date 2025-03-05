using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Dietcode.Database.Orm.Context
{
    public partial class FrenchContext : DbContext
    {
        public FrenchContext()
        {
        }

        public FrenchContext(DbContextOptions<FrenchContext> options)
            : base(options)
        {

        }

        //public virtual DbSet<ArquivoNotaFiscal> ArquivoNotaFiscal { get; set; }
        //public virtual DbSet<Banco> Bancos { get; set; }
        //public virtual DbSet<Cidade> Cidade { get; set; }
        //public virtual DbSet<Cliente> Cliente { get; set; }
        //public virtual DbSet<ComposicaoNotaFiscal> ComposicaoNotaFiscal { get; set; }
        //public virtual DbSet<Cor> Cor { get; set; }
        //public virtual DbSet<Estado> Estado { get; set; }
        //public virtual DbSet<Faturamento> Faturamento { get; set; }
        //public virtual DbSet<Lead> Lead { get; set; }
        //public virtual DbSet<Mes> Mes { get; set; }
        //public virtual DbSet<NotaFiscal> NotaFiscal { get; set; }
        //public virtual DbSet<Servico> Servico { get; set; }
        //public virtual DbSet<StatusNotaFiscal> StatusNotaFiscal { get; set; }
        //public virtual DbSet<Tarefa> Tarefa { get; set; }
        //public virtual DbSet<TarefaItem> TarefaItem { get; set; }
        //public virtual DbSet<TipoDeCliente> TipoDeCliente { get; set; }
        //public virtual DbSet<TipoDePessoa> TipoDePessoa { get; set; }
        //public virtual DbSet<Tributo> Tributo { get; set; }
        //public virtual DbSet<TributoNotaFiscal> TributoNotaFiscal { get; set; }
        //public virtual DbSet<Usuario> Usuario { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("FrenchContext");

                optionsBuilder.UseSqlServer(connectionString);

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AI");

            //modelBuilder.Entity<ArquivoNotaFiscal>(entity =>
            //{
            //    entity.Property(e => e.Arquivo).IsRequired();

            //    entity.HasOne(d => d.NotaFiscal)
            //        .WithMany(p => p.ArquivoNotaFiscal)
            //        .HasForeignKey(d => d.NotaFiscalId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_ArquivoNotaFiscal_NotaFiscal");
            //});

            //modelBuilder.Entity<Cidade>(entity =>
            //{
            //    entity.Property(e => e.CidadeId).HasComment("Codigo");

            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(100)
            //        .IsUnicode(false)
            //        .HasComment("Nome");

            //    entity.HasOne(d => d.Estado)
            //        .WithMany(p => p.Cidade)
            //        .HasForeignKey(d => d.EstadoId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_Cidade_Estado");
            //});

            //modelBuilder.Entity<Cliente>(entity =>
            //{
            //    entity.Property(e => e.Bairro)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.CadastroMunicipal)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Cep)
            //        .HasMaxLength(20)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Complemento)
            //        .HasMaxLength(20)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Contato)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.DataAlteracao).HasColumnType("datetime");

            //    entity.Property(e => e.DataCriacao)
            //        .HasColumnType("datetime")
            //        .HasDefaultValueSql("(getdate())");

            //    entity.Property(e => e.Documento)
            //        .IsRequired()
            //        .HasMaxLength(30)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Email)
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Endereco)
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.InscricaoEstadual)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Numero)
            //        .HasMaxLength(5)
            //        .IsUnicode(false);

            //    entity.Property(e => e.RazaoSocial)
            //        .IsRequired()
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Telefone)
            //        .HasMaxLength(20)
            //        .IsFixedLength(true);

            //    entity.Property(e => e.EstadoId).HasColumnType("int");

            //    entity.HasOne(d => d.Cidade)
            //        .WithMany(p => p.Cliente)
            //        .HasForeignKey(d => d.CidadeId)
            //        .HasConstraintName("FK_Cliente_Cidade");

            //    entity.HasOne(d => d.TipoDeCliente)
            //        .WithMany(p => p.Cliente)
            //        .HasForeignKey(d => d.TipoDeClienteId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_Cliente_TipoDeCliente");

            //    entity.HasOne(d => d.TipoDePessoa)
            //        .WithMany(p => p.Cliente)
            //        .HasForeignKey(d => d.TipoDePessoaId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_Cliente_TipoDePessoa");
            //});

            //modelBuilder.Entity<Lead>(entity =>
            //{
            //    entity.Property(e => e.LeadId).ValueGeneratedOnAdd(); ;

            //    entity.Property(e => e.Descricao)
            //        .HasMaxLength(150)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Empresa)
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Contato)
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Documento)
            //        .HasMaxLength(20)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Telefone)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Email)
            //        .HasMaxLength(150)
            //        .IsUnicode(false);

            //    entity.Property(e => e.DataDeCriacao)
            //        .HasColumnType("datetime")
            //        .HasDefaultValueSql("(getdate())");

            //    entity.Property(e => e.Documento)
            //        .IsRequired()
            //        .HasMaxLength(30)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Email)
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.UsuarioId).HasColumnType("int").IsRequired();

            //    entity.Property(e => e.TipoDePessoaId).HasColumnType("int").IsRequired();

            //    entity.Property(e => e.TipoDeClienteId).HasColumnType("int").IsRequired();

            //    entity.Property(e => e.Efetivada).HasColumnType("bit").IsRequired();

            //});

            //modelBuilder.Entity<ComposicaoNotaFiscal>(entity =>
            //{
            //    entity.Property(e => e.Total).HasColumnType("numeric(18, 2)");

            //    entity.HasOne(d => d.NotaFiscal)
            //        .WithMany(p => p.ComposicaoNotaFiscal)
            //        .HasForeignKey(d => d.NotaFiscalId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_ComposicaoNotaFiscal_NotaFiscal");

            //    entity.HasOne(d => d.Tarefa)
            //        .WithMany(p => p.ComposicaoNotaFiscal)
            //        .HasForeignKey(d => d.TarefaId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_ComposicaoNotaFiscal_Tarefa");
            //});

            //modelBuilder.Entity<Cor>(entity =>
            //{
            //    entity.Property(e => e.CorId).HasComment("Codigo");

            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false)
            //        .HasComment("Descricao");
            //});

            //modelBuilder.Entity<Estado>(entity =>
            //{
            //    entity.Property(e => e.NomeUf)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false)
            //        .HasComment("Descricao");

            //    entity.Property(e => e.SiglaUf)
            //        .IsRequired()
            //        .HasMaxLength(2)
            //        .IsUnicode(false)
            //        .HasComment("UF");
            //});

            //modelBuilder.Entity<Faturamento>(entity =>
            //{
            //    entity.Property(e => e.Valor).HasColumnType("numeric(18, 2)");

            //    entity.HasOne(d => d.Cliente)
            //        .WithMany(p => p.Faturamento)
            //        .HasForeignKey(d => d.ClienteId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_Faturamento_Cliente");
            //});

            //modelBuilder.Entity<Mes>(entity =>
            //{
            //    entity.Property(e => e.MesId).HasComment("Numero");

            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(15)
            //        .IsUnicode(false)
            //        .HasComment("Nome");
            //});

            //modelBuilder.Entity<NotaFiscal>(entity =>
            //{
            //    entity.Property(e => e.CodigoVerificacao)
            //        .HasMaxLength(100)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Data).HasColumnType("date");

            //    entity.Property(e => e.Descricao)
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.ImpostoTotalRetido).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.Numero)
            //        .IsRequired()
            //        .HasMaxLength(10)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Valor).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.ValorLiquido).HasColumnType("numeric(18, 2)");

            //    entity.HasOne(d => d.Cliente)
            //        .WithMany(p => p.NotaFiscal)
            //        .HasForeignKey(d => d.ClienteId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_NotaFiscal_Cliente");

            //    entity.HasOne(d => d.StatusNotaFiscal)
            //        .WithMany(p => p.NotaFiscal)
            //        .HasForeignKey(d => d.StatusNotaFiscalId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_NotaFiscal_StatusNotaFiscal");
            //});

            //modelBuilder.Entity<Servico>(entity =>
            //{
            //    entity.Property(e => e.Descricao)
            //        .IsRequired()
            //        .HasMaxLength(75)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(30)
            //        .IsUnicode(false);
            //});

            //modelBuilder.Entity<StatusNotaFiscal>(entity =>
            //{
            //    entity.Property(e => e.StatusNotaFiscalId).ValueGeneratedOnAdd();

            //    entity.Property(e => e.Descricao)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false);
            //});

            //modelBuilder.Entity<Tarefa>(entity =>
            //{
            //    entity.Property(e => e.DataFim).HasColumnType("date");

            //    entity.Property(e => e.DataInicio).HasColumnType("date");

            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.Observacao)
            //        .HasMaxLength(250)
            //        .IsUnicode(false);

            //    entity.Property(e => e.TotalHoras).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.ValorCobrado).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.ValorHora).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.ValorOrcado).HasColumnType("numeric(18, 2)");

            //    entity.HasOne(d => d.Cliente)
            //        .WithMany(p => p.Tarefa)
            //        .HasForeignKey(d => d.ClienteId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_Tarefa_Cliente");
            //});

            //modelBuilder.Entity<TarefaItem>(entity =>
            //{
            //    entity.Property(e => e.Data).HasColumnType("date");

            //    entity.Property(e => e.Descricao)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false);

            //    entity.Property(e => e.HorasGastas).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.HorasOrcadas).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.ValorHora).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.ValorItem).HasColumnType("numeric(18, 2)");

            //    entity.HasOne(d => d.Servico)
            //        .WithMany(p => p.TarefaItem)
            //        .HasForeignKey(d => d.ServicoId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_TarefaItem_Servico");

            //    entity.HasOne(d => d.Tarefa)
            //        .WithMany(p => p.TarefaItem)
            //        .HasForeignKey(d => d.TarefaId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_TarefaItem_Tarefa");
            //});

            //modelBuilder.Entity<TipoDeCliente>(entity =>
            //{
            //    entity.Property(e => e.TipoDeClienteId).ValueGeneratedOnAdd();

            //    entity.Property(e => e.Descricao)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false);
            //});

            //modelBuilder.Entity<TipoDePessoa>(entity =>
            //{
            //    entity.Property(e => e.TipoDePessoaId).ValueGeneratedOnAdd();

            //    entity.Property(e => e.Descricao)
            //        .IsRequired()
            //        .HasMaxLength(50)
            //        .IsUnicode(false);
            //});

            //modelBuilder.Entity<Tributo>(entity =>
            //{
            //    entity.Property(e => e.TributoId).HasComment("Codigo");

            //    entity.Property(e => e.Descricao)
            //        .IsRequired()
            //        .HasMaxLength(100)
            //        .IsUnicode(false)
            //        .HasComment("Descricao");

            //    entity.Property(e => e.FaixaFinal).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.FaixaInicial).HasColumnType("numeric(18, 2)");

            //    entity.Property(e => e.Percentual)
            //        .HasColumnType("numeric(18, 2)")
            //        .HasComment("Valor");

            //    entity.Property(e => e.ValorDeducao).HasColumnType("numeric(18, 2)");
            //});

            //modelBuilder.Entity<TributoNotaFiscal>(entity =>
            //{
            //    entity.Property(e => e.Total).HasColumnType("numeric(18, 2)");

            //    entity.HasOne(d => d.NotaFiscal)
            //        .WithMany(p => p.TributoNotaFiscal)
            //        .HasForeignKey(d => d.NotaFiscalId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_TributoNotaFiscal_NotaFiscal");

            //    entity.HasOne(d => d.Tributo)
            //        .WithMany(p => p.TributoNotaFiscal)
            //        .HasForeignKey(d => d.TributoId)
            //        .OnDelete(DeleteBehavior.ClientSetNull)
            //        .HasConstraintName("FK_TributoNotaFiscal_Tributo");
            //});

            //modelBuilder.Entity<Usuario>(entity =>
            //{
            //    entity.Property(e => e.Login).IsRequired();
            //    entity.Property(e => e.Nome).IsRequired();
            //    entity.Property(e => e.Email).IsRequired();
            //    entity.Property(e => e.Celular).IsRequired();
            //    entity.Property(e => e.Senha).IsRequired();
            //    entity.Property(e => e.DataCriacao).IsRequired();
            //    entity.Property(e => e.DataAtualizacao);
            //    entity.Property(e => e.ValidadeSenha).IsRequired();
            //    entity.Property(e => e.Admin).IsRequired();
            //    entity.Ignore(e => e.SenhaText);
            //});

            //modelBuilder.Entity<Banco>(entity =>
            //{
            //    entity.Property(e => e.Codigo)
            //        .IsRequired()
            //        .HasMaxLength(10)
            //        .IsUnicode(false);
            //    entity.Property(e => e.Nome)
            //        .IsRequired()
            //        .HasMaxLength(150)
            //        .IsUnicode(false);
            //});

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
