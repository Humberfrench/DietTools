# Dietcode.Database.Net.Orm

Implementação legada, com **Entity Framework 6** e **Dapper**, dos contratos definidos em `Dietcode.Database.Net.Domain`, voltada a aplicações **ASP.NET clássicas** em .NET Framework 4.8 (`System.Web` / `HttpContext.Current`).

É o equivalente legado de `Dietcode.Database.Orm`: mesma responsabilidade (repositório + unit of work + contexto), mas usando EF6 em vez de EF Core e escopo por `HttpContext.Current` em vez de `AsyncLocal`.

Este projeto usa o formato clássico de `.csproj` com `packages.config` e **não é publicado como pacote NuGet**: é referenciado via `ProjectReference` a `Dietcode.Database.Net.Domain` dentro da solução.

## Funcionalidades

- `ThisDatabase`: `DbContext` (EF6) abstrato, que lê a connection string nomeada `ThisDatabaseContext` via `Credpay.Tools.Library.AppSettings`, com `LazyLoadingEnabled` e `ProxyCreationEnabled` desativados.
- `ThisDatabase<Table>`: `DbContext` genérico com um `DbSet<Table>` (`TableData`) para uma única entidade.
- `MyContextManager<T>`: implementação de `IMyContextManager<T>` que guarda a instância do contexto em `HttpContext.Current.Items` (uma por requisição); fora de um contexto HTTP, cria uma nova instância a cada chamada.
- `BaseRepository<TEntity>`: implementação de `IBaseRepository<TEntity>` com `Adicionar`, `Atualizar`, `Remover`, `ObterPorId`, `ObterTodos`, `ObterTodosPaginado` e `Pesquisar`, além de uma propriedade `Connection` (`IDbConnection`/`SqlConnection`) pronta para uso com Dapper.
- `MyUnitOfWork<T>`: implementação de `IMyUnitOfWork<T>` com `SaveChanges()` síncrono, que percorre até dois níveis de `InnerException` de erros de atualização do EF6 e os acumula em um `ValidationResult<T>`.

> Observação: por herança do código-fonte, `BaseRepository<TEntity>` está fisicamente neste projeto mas declarado no namespace `Dietcode.Database.Net.Domain`, e `MyUnitOfWork<T>` no namespace `Dietcode.Database.Net.Domain.UnitOfWork`. Os `using` abaixo refletem isso.

## Contexto e repositório

```csharp
using Dietcode.Database.Net.Domain;
using Dietcode.Database.Net.Orm.Context;

public class UserRepository : BaseRepository<User>
{
    public UserRepository(IMyContextManager<ThisDatabase<User>> contextManager)
        : base(contextManager)
    {
    }
}

var contextManager = new MyContextManager<ThisDatabase<User>>();
var repository = new UserRepository(contextManager);

var usuario = await repository.ObterPorId(1);
var ativos = await repository.Pesquisar(u => u.Ativo);
var pagina = await repository.ObterTodosPaginado(pagina: 1, registros: 20);

await repository.Adicionar(new User { Nome = "Maria" });
```

`ObterPorId` retorna `new TEntity()` (nunca `null`) quando o registro não é encontrado.

## Unit of Work

```csharp
using Dietcode.Database.Net.Domain.UnitOfWork;

var unitOfWork = new MyUnitOfWork<User>(contextManager);

await repository.Adicionar(new User { Nome = "Maria" });

ValidationResult<User> resultado = unitOfWork.SaveChanges();
if (resultado.Invalid)
{
    // tratar resultado.Erros
}
```

## Consulta direta via Dapper

```csharp
using Dapper;

var usuarios = (await repository.Connection.QueryAsync<User>(
    "SELECT * FROM Users WHERE Ativo = 1")).ToList();
```

## Configuração da connection string

A connection string é lida pela chave `ThisDatabaseContext` (via `Credpay.Tools.Library.AppSettings`), configurada no `web.config`/`app.config` da aplicação consumidora — este projeto em si só traz configuração de binding redirects e do provider do Entity Framework 6 (`App.config`).

## Pacotes relacionados

- `Dietcode.Database.Net.Domain`: define os contratos (`IBaseRepository`, `IMyUnitOfWork`, `IMyContextManager`) implementados por este pacote.
- `Dietcode.Database.Orm`: equivalente moderno (Entity Framework Core, net10.0), com Unit of Work assíncrono e escopo agnóstico de host via `IAmbientContextStore`.

## Licença

MIT
