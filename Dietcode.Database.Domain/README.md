# Dietcode.Database.Domain

Contratos (interfaces) de repositório, unit of work e contexto ambiente usados pela família de acesso a dados baseada em Entity Framework Core para .NET moderno. Este pacote não contém implementação: ele define o "shape" que `Dietcode.Database.Orm` implementa com EF Core.

É o equivalente moderno (net10.0) de `Dietcode.Database.Net.Domain`, que cumpre o mesmo papel para projetos legados em .NET Framework 4.8.

## Instalação

```bash
dotnet add package Dietcode.Database.Domain --version 10.5.0
```

## Funcionalidades

- `IBaseRepository<Table, Tipo>`: contrato de repositório genérico assíncrono, com CRUD, paginação, pesquisa por predicado (com ou sem `Include`), contagem, verificação de existência e inserção em lote.
- `IMyUnitOfWork<T>`: contrato de unit of work, com `BeginTransaction()` e `SaveChanges` retornando `ValidationResult<T>` (de `Dietcode.Core.DomainValidator`).
- `IMyContextManager<ContextT>`: contrato para obtenção do contexto de dados corrente (`GetContext()`), permitindo compartilhar a mesma instância dentro de um escopo (requisição, worker etc.).
- `IAmbientContextStore`: contrato de um armazenamento ambiente agnóstico de host (`TryGet`, `Set`, `BeginScope`), usado para guardar o contexto corrente sem depender de `HttpContext`.
- `ICompositeKey`: contrato para entidades com chave composta, expondo os valores da chave via `Values()`.
- `Entries`: par simples `EntryName` / `EntryKeyValue`, usado para registrar identificadores de entidades afetadas por uma operação de persistência.

## IBaseRepository

```csharp
public interface IBaseRepository<Table, Tipo> : IDisposable
    where Table : class, new()
{
    Task<bool> Adicionar(Table obj, CancellationToken ct = default);
    Task<bool> Atualizar(Table obj, CancellationToken ct = default);
    Task<bool> Remover(Table obj, CancellationToken ct = default);

    Task<Table?> ObterPorId(Tipo id, bool asTracking = false, CancellationToken ct = default);
    Task<List<Table>> ObterTodos(bool asTracking = false, CancellationToken ct = default);
    Task<List<Table>> ObterTodos(int pagina, int registros, CancellationToken ct = default);
    Task<IEnumerable<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default);
    Task<List<Table>> Pesquisar(Expression<Func<Table, bool>> predicate, CancellationToken ct = default,
                                 params Expression<Func<Table, object>>[] includes);

    Task<bool> Existe(Expression<Func<Table, bool>> predicate, CancellationToken ct = default);
    Task<int> Contar(Expression<Func<Table, bool>>? predicate = null, CancellationToken ct = default);
    Task<int> AdicionarRange(IEnumerable<Table> itens, CancellationToken ct = default);

    void BeginTransaction();
    Task<ValidationResult<Table>> Commit(CancellationToken ct = default);
}
```

`Tipo` é o tipo da chave primária: um tipo simples (`int`, `Guid` etc.) ou um tipo que implementa `ICompositeKey` para chaves compostas.

## ICompositeKey

```csharp
public readonly record struct PedidoItemKey(int PedidoId, int ItemId) : ICompositeKey
{
    public object[] Values() => new object[] { PedidoId, ItemId };
}
```

A implementação de `IBaseRepository<Table, Tipo>` usa `Values()` para localizar a entidade por chave composta (por exemplo, via `DbSet.FindAsync`).

## IMyUnitOfWork e IMyContextManager

```csharp
public interface IMyUnitOfWork<T> where T : class, new()
{
    void BeginTransaction();
    Task<ValidationResult<T>> SaveChanges(CancellationToken ct = default);
}

public interface IMyContextManager<ContextT>
{
    ContextT GetContext();
}
```

## IAmbientContextStore

```csharp
public interface IAmbientContextStore
{
    bool TryGet<T>(string key, out T value);
    void Set(string key, object value);

    // Cria um "escopo" lógico. Útil para Web (1 por request) e Worker (1 por execução).
    IDisposable BeginScope();
}
```

## Pacotes relacionados

- `Dietcode.Database.Orm`: implementa estes contratos com Entity Framework Core (SQL Server), incluindo `AmbientContextStore`, `MyContextManager<T>`, `MyUnitOfWork<T>` e `BaseRepository<Table, Tipo>`.
- `Dietcode.Core.DomainValidator`: fornece `ValidationResult<T>`, usado nas assinaturas de `IBaseRepository` e `IMyUnitOfWork`.
- `Dietcode.Database.Net.Domain`: contratos equivalentes para projetos legados em .NET Framework 4.8.

## Licença

MIT
