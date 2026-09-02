# Dietcode.Database.Orm

Implementação com **Entity Framework Core** (SQL Server) dos contratos definidos em `Dietcode.Database.Domain`: repositório genérico, unit of work, gerenciamento de contexto e logging estruturado com Serilog. É a peça de implementação da família moderna (net10.0); `Dietcode.Database.Domain` define apenas as interfaces.

Ao contrário de `Dietcode.Database.Classic`, que expõe um `DbContext` simplificado e independente, este pacote é acoplado aos contratos `IBaseRepository<Table, Tipo>` / `IMyUnitOfWork<T>` / `IMyContextManager<ContextT>` e pensado para ser registrado via injeção de dependência com `Builder.BuilderStart`.

## Instalação

```bash
dotnet add package Dietcode.Database.Orm --version 10.5.0
```

## Funcionalidades

- `Builder.BuilderStart(IServiceCollection services)`: registra `IBaseRepository<,>` → `BaseRepository<,>`, `IMyContextManager<>` → `MyContextManager<>`, `IMyUnitOfWork<>` → `MyUnitOfWork<>` (todos `Scoped`) e `IAmbientContextStore` → `AmbientContextStore` (`Singleton`).
- `ThisDatabase` / `ThisDatabase<T1>`: `DbContext` (SQL Server) que lê a connection string `DbContextConnString` de `appsettings.json`, com `EnableSensitiveDataLogging`, `EnableDetailedErrors`, `EnableServiceProviderCaching` e um `DbSet<T1>` por entidade.
- `BaseRepository<Table, Tipo>`: implementação assíncrona de `IBaseRepository<Table, Tipo>` com suporte a chave simples ou composta (`ICompositeKey`), paginação, pesquisa com `Include`, `Existe`, `Contar`, `AdicionarRange` e integração com o unit of work (`BeginTransaction` / `Commit`).
- Consultas SQL diretas via Dapper na mesma connection string do `DbContext`: `QueryListAsync<T>`, `QueryFirstOrDefaultAsync<T>`, `ExecuteAsync` e `QueryFirstOrDefaultFromStoredProcedureAsync<T>` (protegidos, disponíveis para classes que herdam de `BaseRepository<Table, Tipo>`).
- `MyUnitOfWork<T>`: `SaveChanges` assíncrono que devolve `ValidationResult<T>`, populando `Entries` com as chaves primárias (simples ou compostas) das entidades afetadas, e registrando cada etapa via Serilog.
- `AmbientContextStore`: implementação de `IAmbientContextStore` baseada em `AsyncLocal`, permitindo compartilhar o mesmo `DbContext` dentro de um escopo lógico (`BeginScope()`), tanto em aplicações web quanto em workers, sem depender de `HttpContext`.
- `MyContextManager<T>`: obtém (ou cria e armazena no `IAmbientContextStore`) a instância de contexto corrente, usando o nome completo do tipo como chave.
- Logging estruturado das operações de EF Core e do repositório via Serilog (`InternalOrmLoggerFactory`, `EfPerformanceObserver`), gravando em `logs/orm-log-.json` (JSON compacto, rotação diária) e no console.
- `Orm:EnableLogging` em `appsettings.json` liga/desliga o logging (habilitado por padrão).

## Registro via DI

```csharp
using Dietcode.Database.Orm;

var builder = WebApplication.CreateBuilder(args);

Builder.BuilderStart(builder.Services);

var app = builder.Build();
```

## Repositório

```csharp
public class UserService
{
    private readonly IBaseRepository<User, int> _repository;

    public UserService(IBaseRepository<User, int> repository) => _repository = repository;

    public Task<User?> GetAsync(int id, CancellationToken ct) =>
        _repository.ObterPorId(id, asTracking: false, ct);

    public async Task<ValidationResult<User>> CreateAsync(User user, CancellationToken ct)
    {
        await _repository.Adicionar(user, ct);
        return await _repository.Commit(ct);
    }
}
```

Para chave composta, `Tipo` deve implementar `ICompositeKey`:

```csharp
public readonly record struct PedidoItemKey(int PedidoId, int ItemId) : ICompositeKey
{
    public object[] Values() => new object[] { PedidoId, ItemId };
}

public class PedidoItemService
{
    private readonly IBaseRepository<PedidoItem, PedidoItemKey> _repository;

    public PedidoItemService(IBaseRepository<PedidoItem, PedidoItemKey> repository) => _repository = repository;

    public Task<PedidoItem?> GetAsync(PedidoItemKey id, CancellationToken ct) =>
        _repository.ObterPorId(id, ct: ct);
}
```

## Consultas Dapper dentro do repositório

Uma classe que herda de `BaseRepository<Table, Tipo>` tem acesso direto a métodos protegidos para SQL cru ou stored procedures, usando a mesma connection string do `DbContext`:

```csharp
public class UserRepository : BaseRepository<User, int>
{
    public UserRepository(IMyContextManager<ThisDatabase<User>> contextManager) : base(contextManager) { }

    public Task<List<User>> ObterAtivosAsync(CancellationToken ct) =>
        QueryListAsync<User>("SELECT * FROM Users WHERE Active = 1", null, ct);
}
```

## Configuração da connection string

```json
{
  "ConnectionStrings": {
    "DbContextConnString": "Server=.;Database=Dietcode;Trusted_Connection=True;"
  },
  "Orm": {
    "EnableLogging": true
  }
}
```

Se `DbContextConnString` não estiver definida, `ThisDatabase` lança `ArgumentException` ao ser construído.

## Pacotes relacionados

- `Dietcode.Database.Domain`: define os contratos (`IBaseRepository`, `IMyUnitOfWork`, `IMyContextManager`, `IAmbientContextStore`, `ICompositeKey`) implementados por este pacote.
- `Dietcode.Core.DomainValidator`: fornece `ValidationResult<T>`, retornado por `Commit` e `SaveChanges`.
- `Dietcode.Core.Lib`: usado internamente (extensões de string).
- `Dietcode.Database.Classic`: alternativa mais simples, com `DbContext` por entidade e sem os contratos de `Dietcode.Database.Domain`.
- `Dietcode.Database.Net.Orm`: equivalente legado (Entity Framework 6) para projetos .NET Framework 4.8.

## Licença

MIT
