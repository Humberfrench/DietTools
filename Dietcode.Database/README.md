# Dietcode.Database

Infraestrutura leve e **async-first** para acesso a dados com **Dapper** e **Dapper.Contrib**, focada em APIs modernas (.NET / ASP.NET Core), com suporte a múltiplos bancos, **logging estruturado em JSON com mask de dados sensíveis**, e integração limpa via **Dependency Injection**.

---

## ✨ Principais Características

- ✅ **Async-only** (evita deadlocks e thread starvation)
- ✅ Baseado em **Dapper** (alto desempenho)
- ✅ Suporte a **SQL Server, PostgreSQL, MySQL e Oracle**
- ✅ **Repositório genérico** simples e honesto
- ✅ **Unit of Work** explícito para transações
- ✅ Logging estruturado em **JSON Lines (.jsonl)**
- ✅ **Mask automático** de dados sensíveis (senha, token, etc.)
- ✅ Extensível via **Decorator Pattern** (logging, cache, retry)
- ✅ Pronto para uso como **NuGet corporativo**

---

## 📦 Instalação

```bash
dotnet add package Dietcode.Database
```

Pacotes transitivos utilizados:
- `Dapper`
- `Dapper.Contrib`
- Provider ADO.NET do banco escolhido (ex.: `Microsoft.Data.SqlClient`)

---

## 🧱 Conceitos Importantes

### 🔹 Async-only

Este pacote **não expõe métodos síncronos** por decisão arquitetural.

Motivos:
- ASP.NET Core é async-first
- Evita bloqueio de threads
- Melhor escalabilidade

---

### 🔹 Separação de Responsabilidades

- **Factories**: criam conexões
- **Repository**: executa CRUD e queries
- **UnitOfWork**: controla transações
- **Logging**: adicionado por *decorators*, não por herança

---

## 🗄️ Configuração por Banco

### SQL Server

```csharp
builder.Services.AddDietcodeSqlServer(
    builder.Configuration.GetConnectionString("Default"));
```

### PostgreSQL

```csharp
builder.Services.AddDietcodePostgreSql(
    builder.Configuration.GetConnectionString("Default"));
```

### MySQL

```csharp
builder.Services.AddDietcodeMySql(
    builder.Configuration.GetConnectionString("Default"));
```

### Oracle

```csharp
builder.Services.AddDietcodeOracle(
    builder.Configuration.GetConnectionString("Default"));
```

> 🔎 O repositório **não sabe qual banco está sendo usado**.
> A escolha é feita exclusivamente via DI.

---

## 🧩 Uso do Repositório

```csharp
public class UserService
{
    private readonly IRepository<User> _repository;

    public UserService(IRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<User?> GetAsync(int id, CancellationToken ct)
    {
        return await _repository.GetByIdAsync(id, ct);
    }
}
```

---

## 🔄 Unit of Work (Transações)

```csharp
await unitOfWork.ExecuteAsync(async (conn, tx) =>
{
    await conn.ExecuteAsync(
        "INSERT INTO Users (Name) VALUES (@Name)",
        new { Name = "John" },
        tx);
});
```

✔ Commit automático em sucesso
✔ Rollback automático em exceção

---

## 🏷️ Atributos de Mapeamento (Dapper.Contrib)

O pacote fornece wrappers semânticos para atributos do Dapper:

```csharp
[TableName("users")]
public class User
{
    [KeyId]
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    [WriteCol(false)]
    public string PasswordHash { get; set; } = string.Empty;
}
```

Esses atributos:
- Evitam acoplamento direto ao Dapper
- Facilitam futura troca de ORM

---

## 🪵 Logging JSON + Mask (Opcional)

### Ativando o logging

```csharp
services.AddScoped<IRepositoryLogger, JsonRepositoryLogger>();
services.Decorate(typeof(IRepository<>), typeof(LoggingRepositoryDecorator<>));
```

### Exemplo de log gerado (`.jsonl`)

```json
{
  "timestamp": "2026-01-16T14:33:21Z",
  "operation": "GetById",
  "context": { "id": 10 },
  "durationMs": 12.4
}
```

### Mask automático

Campos mascarados por padrão:
- `password`
- `senha`
- `token`
- `accessToken`
- `refreshToken`

---

## 🧠 Extensibilidade

O pacote foi desenhado para **composição**, não herança.

Você pode adicionar facilmente:

- 🔁 Retry (Polly)
- 📦 Cache
- 📊 Métricas
- 🧪 Tracing distribuído

Tudo via **decorators**.

---

## 🚫 O que este pacote NÃO faz

- ❌ Não implementa LINQ
- ❌ Não faz tracking de entidades
- ❌ Não escolhe banco por enum ou switch
- ❌ Não executa queries síncronas

---

## 📄 Licença

MIT

---

## 🤝 Contribuição

Pull requests são bem-vindos.

Antes de contribuir:
- Respeite o padrão async-only
- Evite herança (prefira composição)
- Não adicione lógica de negócio ao repositório

