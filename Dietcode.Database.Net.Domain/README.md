# Dietcode.Database.Net.Domain

Contratos (interfaces) de repositório, unit of work e contexto usados pela família de acesso a dados legada, para projetos em **.NET Framework 4.8**. É o equivalente de `Dietcode.Database.Domain`, porém mais simples e voltado a aplicações ASP.NET clássicas (Web Forms / MVC sobre `System.Web`).

Este projeto usa o formato clássico de `.csproj` com `packages.config` e **não é publicado como pacote NuGet**: é referenciado diretamente via `ProjectReference` dentro da solução (por `Dietcode.Database.Net.Orm`).

## Funcionalidades

- `IBaseRepository<TEntity>`: contrato de repositório assíncrono, sem suporte a chave composta e sem `CancellationToken` — mais simples que o `IBaseRepository<Table, Tipo>` da versão moderna. Chave primária sempre `int`.
- `IMyContextManager<ContextT>`: contrato para obtenção do contexto de dados corrente (`GetContext()`).
- `IMyUnitOfWork<T>`: contrato de unit of work síncrono, com `BeginTransaction()` e `SaveChanges()` retornando `ValidationResult<T>` de `Credpay.Tools.DomainValidator` (não de `Dietcode.Core.DomainValidator`).

## IBaseRepository

```csharp
public interface IBaseRepository<TEntity> : IDisposable
{
    Task<bool> Adicionar(TEntity obj);
    Task<bool> Atualizar(TEntity obj);
    Task<bool> Remover(TEntity obj);

    Task<TEntity> ObterPorId(int id);
    Task<IEnumerable<TEntity>> ObterTodos();
    Task<IEnumerable<TEntity>> ObterTodosPaginado(int pagina, int registros);
    Task<IEnumerable<TEntity>> Pesquisar(Expression<Func<TEntity, bool>> predicate);
}
```

## IMyUnitOfWork e IMyContextManager

```csharp
public interface IMyUnitOfWork<T> where T : class, new()
{
    void BeginTransaction();
    ValidationResult<T> SaveChanges();
}

public interface IMyContextManager<ContextT>
{
    ContextT GetContext();
}
```

## Diferenças em relação a Dietcode.Database.Domain

- Não existe equivalente a `ICompositeKey`, `IAmbientContextStore` nem `Entries` — chave sempre simples (`int`) e sem um contexto ambiente agnóstico de host.
- `SaveChanges()` é síncrono, e o `ValidationResult<T>` retornado vem de `Credpay.Tools.DomainValidator`, não de `Dietcode.Core.DomainValidator`.
- `ObterTodosPaginado` substitui a sobrecarga paginada de `ObterTodos` da versão moderna.

## Pacotes relacionados

- `Dietcode.Database.Net.Orm`: implementa estes contratos com Entity Framework 6 e Dapper, usando `HttpContext.Current` para escopo por requisição.
- `Dietcode.Database.Domain`: contratos equivalentes para a família moderna baseada em .NET (net10.0) e Entity Framework Core.

## Licença

MIT
