# Dietcode.Database.Classic

Wrapper simplificado sobre **Entity Framework Core** (SQL Server) com um `DbContext` por entidade e um repositório genérico com CRUD assíncrono básico, além de consultas SQL diretas via Dapper na mesma conexão. Não usa os contratos de `Dietcode.Database.Domain` (`IBaseRepository`, `IMyUnitOfWork` etc.) — é uma alternativa autônoma e mais enxuta, pensada para cenários simples que não precisam de Unit of Work explícito nem de contexto ambiente compartilhado.

Para o cenário completo, com contratos, chave composta, paginação e Unit of Work, veja `Dietcode.Database.Orm`.

## Instalação

```bash
dotnet add package Dietcode.Database.Classic --version 10.2.0
```

## Funcionalidades

- `Database`: `DbContext` abstrato para SQL Server, configurado via `Database.Configure(connectionString, version)`, com retry automático (`EnableRetryOnFailure`), `EnableSensitiveDataLogging` e precisão decimal padrão `(18, 8)`.
- `SQLVersion`: enum (`SQL2019`, `SQL2016`, `SQL2012`) que ajusta o comportamento de conexão conforme a versão do SQL Server.
- `Database<T>`: `DbContext` genérico que registra uma única entidade `T` no modelo (`OnModelCreating`).
- `BaseRepository<T>`: repositório com `Adicionar`, `Atualizar`, `Remover`, `LoadAll`, `Get(id)`, `Get(predicate)`, `SaveChangesAsync`, além de `Query(sql)` (Dapper) e controle manual de transação (`BeginTransaction`, `Commit`, `RoolBack`).

## Configuração

```csharp
using Dietcode.Database.Classic;

Database.Configure(
    connectionString: builder.Configuration.GetConnectionString("Default"),
    version: Database.SQLVersion.SQL2019);
```

`Configure` define a connection string e a versão do SQL Server usadas por todas as instâncias de `Database<T>` criadas sem connection string explícita.

## Repositório

```csharp
public class User { public int Id { get; set; } public string Email { get; set; } = string.Empty; }

var repository = new BaseRepository<User>
{
    SaveAuto = true // faz commit automático em Adicionar()
};

var novoId = await repository.Adicionar(new User { Email = "user@exemplo.com" });

var usuario = await repository.Get(1);
var ativos = await repository.Get(u => u.Email != null);
var todos = await repository.LoadAll();

await repository.Atualizar(usuario!);
await repository.Remover(usuario!);
```

`SaveAuto` controla se `Adicionar` chama `SaveChangesAsync` automaticamente; `Atualizar` e `Remover` apenas marcam a entidade no `ChangeTracker` — é preciso chamar `SaveChangesAsync()` (ou usar uma transação) para persistir.

Em caso de exceção, a mensagem (incluindo até três níveis de `InnerException`) fica disponível na propriedade `Erro` do repositório.

## Transação manual

```csharp
var tx = await repository.BeginTransaction();
try
{
    await repository.Adicionar(new User { Email = "a@exemplo.com" });
    await repository.SaveChangesAsync();
    await repository.Commit();
}
catch
{
    await repository.RoolBack();
    throw;
}
```

## Consulta direta via Dapper

```csharp
List<User> usuarios = await repository.Query("SELECT * FROM Users WHERE Active = 1");
```

`BaseRepository<T>` expõe `Connection` (`IDbConnection`) e `ConnectionString`, usados internamente pelo método `Query`, que executa SQL diretamente com Dapper na mesma conexão do repositório.

## Pacotes relacionados

- `Dietcode.Database.Orm`: implementação completa baseada nos contratos de `Dietcode.Database.Domain`, com Unit of Work, chave composta e logging estruturado.
- `Dietcode.Database`: alternativa baseada apenas em Dapper (sem Entity Framework Core).

## Licença

MIT
