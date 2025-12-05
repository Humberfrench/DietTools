# Dietcode.Database Lib

Lib de facilitações de banco de dados para projetos com dapper Crud..

---

# Uso
Para usar a biblioteca Dietcode.Database, siga os passos abaixo:
Na classe =>  [TableName("Nome_Da_Tabela")]

No campoID => [KeyId]

Projeto => app settings => "DataBase": "SqlServer", (ou o que desejar)

Database Enuns
```csharp
    public enum EnumBancos
    {
        SqlServer,
        MySql,
        PostgreSql,
        Oracle
    }


Em Repositorio 



add o ```using Dietcode.Database;
Herde : BaseRepository ```csharp'

```using Dietcode.Database;
public class LogApiEntradaRepository : BaseRepository
    {
        private Repository<LogApiEntrada> repositoryLogApiEntrada;

        public LogApiEntradaRepository() : base()
        {
            repositoryLogApiEntrada = new Repository<LogApiEntrada>(base.ConnectionStringLog, 
                                                                    (EnumBancos)ObterDatabase(Config.DataBase));
        }

        public async Task Add(LogApiEntrada logApiEntrada)
        {
            await repositoryLogApiEntrada.AddAsync(logApiEntrada);
        }
    }