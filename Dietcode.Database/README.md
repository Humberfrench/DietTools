# Dietcode.Database

Infraestrutura leve e assíncrona para acesso a dados com **Dapper** e **Dapper.Contrib**, com suporte a múltiplos bancos (SQL Server, PostgreSQL, MySQL e Oracle), logging estruturado em JSON com mascaramento de dados sensíveis, e registro via Dependency Injection.

Este pacote é independente da família `Dietcode.Database.Domain` / `Dietcode.Database.Orm`: não usa Entity Framework nem os contratos `IBaseRepository` / `IMyUnitOfWork` dessa família. É a opção para quem quer um repositório genérico simples baseado apenas em Dapper, sem tracking de entidades.

## Instalação

```bash
dotnet add package Dietcode.Database --version 10.2.0
```

## Funcionalidades

- Repositório genérico assíncrono (`DapperRepository<T>`) baseado em Dapper e Dapper.Contrib.
- Interfaces `IRepository<T>`, `IReadRepository<T>` e `IWriteRepository<T>`.
- `IConnectionFactory` e fábricas de conexão para SQL Server, PostgreSQL, MySQL e Oracle.
- Extensões de `IServiceCollection` para registrar o repositório e o provider escolhido.
- `DapperUnitOfWork` para execução de comandos dentro de uma transação, com commit/rollback automático.
- Atributos de mapeamento (`KeyIdAttribute`, `ExplicitKeyIdAttribute`, `ComputedColAttribute`, `WriteColAttribute`, `TableNameAttribute`) como wrappers dos atributos do Dapper.Contrib.
- `IRepositoryLogger` e `JsonRepositoryLogger` para logging estruturado em `.jsonl`, com mascaramento automático de dados sensíveis.
- `RepositoryDecorator<T>` / `LoggingRepositoryDecorator<T>` para adicionar logging por composição (Decorator Pattern).

## Configuração por banco

```csharp
builder.Services.AddDietcodeSqlServer(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddDietcodePostgreSql(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddDietcodeMySql(builder.Configuration.GetConnectionString("Default"));
builder.Services.AddDietcodeOracle(builder.Configuration.GetConnectionString("Default"));
```

Cada extensão registra a `IConnectionFactory` correspondente e chama `AddDietcodeDatabase()`, que registra `IRepository<T>` (`DapperRepository<T>`) e `DapperUnitOfWork` como serviços com escopo (`Scoped`). O repositório não sabe qual banco está em uso — a escolha é feita inteiramente via DI.

## Uso do repositório

```csharp
public class UserService
{
    private readonly IRepository<User> _repository;

    public UserService(IRepository<User> repository)
    {
        _repository = repository;
    }

    public Task<User?> GetAsync(int id, CancellationToken ct) =>
        _repository.GetByIdAsync(id, ct);

    public Task<IReadOnlyList<User>> SearchAsync(CancellationToken ct) =>
        _repository.QueryAsync("SELECT * FROM Users WHERE Active = 1", cancellationToken: ct);
}
```

`IReadRepository<T>` expõe `GetByIdAsync`, `GetAllAsync` e `QueryAsync` (SQL livre). `IWriteRepository<T>` expõe `InsertAsync`, `UpdateAsync` e `DeleteAsync`. Todos os métodos exigem chave primária do tipo `int` (via Dapper.Contrib) e são assíncronos — o pacote não expõe métodos síncronos.

## Unit of Work (transações)

```csharp
public class OperationService
{
    private readonly DapperUnitOfWork _unitOfWork;

    public OperationService(DapperUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public Task RegisterAsync(CancellationToken ct) =>
        _unitOfWork.ExecuteAsync(async (conn, tx) =>
        {
            await conn.ExecuteAsync(
                "INSERT INTO Users (Name) VALUES (@Name)",
                new { Name = "John" },
                tx);
        }, ct);
}
```

`ExecuteAsync` abre a conexão, inicia a transação, executa o delegate, faz commit em caso de sucesso e rollback (relançando a exceção) em caso de falha.

## Atributos de mapeamento (Dapper.Contrib)

```csharp
[TableName("users")]
public class User
{
    [KeyId]
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    [WriteCol(false)]
    public string PasswordHash { get; set; } = string.Empty;

    [ComputedCol]
    public int Idade { get; set; }
}
```

`KeyIdAttribute`, `ExplicitKeyIdAttribute`, `ComputedColAttribute`, `WriteColAttribute` e `TableNameAttribute` são subclasses diretas dos atributos equivalentes do `Dapper.Contrib.Extensions`, evitando que o modelo de domínio referencie o Dapper diretamente.

## Logging estruturado (opcional)

`JsonRepositoryLogger` grava cada operação como uma linha JSON no arquivo `db-log.jsonl`, mascarando o contexto com `SensitiveDataMasker` (de `Dietcode.Core.Lib`):

```json
{"timestamp":"2026-01-16T14:33:21Z","operation":"GetById","context":{"id":10},"durationMs":12.4,"error":null}
```

Para ativar o logging, decore a implementação registrada de `IRepository<T>`:

```csharp
services.AddScoped<IRepositoryLogger, JsonRepositoryLogger>();
services.AddScoped<IRepository<User>>(sp =>
    new LoggingRepositoryDecorator<User>(
        new DapperRepository<User>(sp.GetRequiredService<IConnectionFactory>()),
        sp.GetRequiredService<IRepositoryLogger>()));
```

O pacote não inclui um helper de `Decorate(...)` automático (como o do Scrutor); o wrapping precisa ser feito manualmente ou com uma biblioteca de decoração de terceiros. Atualmente `LoggingRepositoryDecorator<T>` instrumenta apenas `GetByIdAsync`; os demais métodos herdam o comportamento de passagem direta de `RepositoryDecorator<T>` sem gerar log.

## Pacotes relacionados

- `Dietcode.Core.Lib`: fornece `SensitiveDataMasker`, usado pelo `JsonRepositoryLogger` para mascarar dados sensíveis no log.
- `Dietcode.Database.Orm` / `Dietcode.Database.Domain`: família alternativa baseada em Entity Framework Core e nos contratos `IBaseRepository` / `IMyUnitOfWork`, para quem precisa de tracking de entidades e Unit of Work orientado a `DbContext`.

## Licença

MIT
