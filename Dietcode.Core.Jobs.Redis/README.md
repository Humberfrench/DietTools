# Dietcode.Core.Jobs.Redis

Implementação via Redis dos contratos de infraestrutura de `Dietcode.Core.Jobs.Interfaces`: `IJobQueue` (fila) e `IAsyncJobStoreGeneric` (store de estado). Faz o mesmo papel que uma implementação em memória faria — fila FIFO e dicionário de estado por `IdempotencyKey` — só que sobrevivendo a restart e compartilhável entre instâncias da aplicação.

## Instalação

```bash
dotnet add package Dietcode.Core.Jobs.Redis --version 10.0.0
```

## Configuração

Via `appsettings.json` + `IConfiguration`:

```json
{
  "RedisJobs": {
    "ConnectionString": "localhost:6379",
    "QueueKey": "dietcode:jobs:queue",
    "StoreKeyPrefix": "dietcode:jobs:state:",
    "DequeuePollingSeconds": 5
  }
}
```

```csharp
using Dietcode.Core.Jobs.Redis.Extensions;

builder.Services.AddDietcodeRedisJobs(builder.Configuration.GetSection("RedisJobs"));
```

Ou via lambda:

```csharp
builder.Services.AddDietcodeRedisJobs(options =>
{
    options.ConnectionString = "meu-redis:6379,password=...";
    options.QueueKey = "minhaapp:jobs:queue";
    options.StoreKeyPrefix = "minhaapp:jobs:state:";
});
```

`AddDietcodeRedisJobs` registra `IJobQueue` e `IAsyncJobStoreGeneric` (ambos via Redis) prontos para o `Dietcode.Core.Jobs` consumir — dispensa `AddSingleton<IJobQueue, ...>`/`AddSingleton<IAsyncJobStoreGeneric, ...>` manuais:

```csharp
using Dietcode.Core.Jobs;
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Redis.Extensions;

builder.Services.AddDietcodeRedisJobs(builder.Configuration.GetSection("RedisJobs"));

builder.Services.AddScoped(typeof(IJobAsyncService<,>), typeof(JobAsyncService<,>));
builder.Services.AddScoped<IJobHandler<GenericJob>, GenericJobHandler>();
builder.Services.AddHostedService<JobWorkerGeneric>();

builder.Services.AddSingleton<IHandlerDispatcher, MeuHandlerDispatcher>();
```

## RedisJobOptions

| Propriedade | Padrão | Descrição |
| --- | --- | --- |
| `ConnectionString` | `"localhost:6379"` | String de conexão do `StackExchange.Redis` (host, senha, TLS etc.). |
| `Database` | `null` | Índice do banco Redis (`null` = padrão do multiplexer, banco 0). |
| `QueueKey` | `"dietcode:jobs:queue"` | Chave da lista Redis usada como fila. |
| `StoreKeyPrefix` | `"dietcode:jobs:state:"` | Prefixo das chaves de estado — cada job vira `{prefixo}{idempotencyKey}`. |
| `DequeuePollingSeconds` | `5` | Timeout (segundos) de cada `BRPOP`. Também é a latência máxima de cancelamento do `DequeueAsync` — ver observação abaixo. |
| `StateTimeToLive` | `null` | TTL do estado do job no Redis. `null` = sem expiração (igual a um dicionário em memória). |

## Implementação — fila (`RedisJobQueue`)

- Lista Redis simples: `EnqueueAsync` faz `LPUSH`, `DequeueAsync` faz `BRPOP` num loop.
- Só guarda a `IdempotencyKey` na lista (não um envelope JSON): hoje o único tipo de job em uso é `GenericJob` (de `Dietcode.Core.Jobs`), que carrega só a `IdempotencyKey` — o worker busca `HandlerKey`/payload no store. Se um segundo tipo de job for introduzido, isso precisa virar um envelope com discriminador de tipo.
- **FIFO simples, at-most-once**: se o worker cair no meio do processamento de um job já retirado da lista (`BRPOP`), esse job se perde — não há redelivery, dead-letter ou consumer group (isso exigiria Redis Streams, fora do escopo desta implementação).
- **Cancelamento tem latência limitada a `DequeuePollingSeconds`**: o `StackExchange.Redis` não cancela um comando `BRPOP` em voo, então o `CancellationToken` só é observado entre uma tentativa de `BRPOP` e a próxima.
- **Atenção ao `syncTimeout`/`asyncTimeout` da `ConnectionString`**: o `StackExchange.Redis` tem seu próprio timeout de cliente (padrão ~5s) e lança `RedisTimeoutException` se o servidor não responder dentro dele — isso é independente do timeout do `BRPOP`. Como `DequeuePollingSeconds` também é 5s por padrão, os dois podem colidir. Configure o timeout do cliente **maior** que `DequeuePollingSeconds` na própria `ConnectionString`, por exemplo: `"localhost:6379,syncTimeout=15000,asyncTimeout=15000"` para `DequeuePollingSeconds = 5`.

## Implementação — store (`RedisJobStore`)

- Cada job vira uma chave String no Redis (`{StoreKeyPrefix}{idempotencyKey}`) guardando o `AsyncJobStateGeneric` serializado em JSON.
- `SetCompletedAsync`/`SetFailedAsync` fazem *read-modify-write* (não são atômicos): leem o JSON, alteram os campos relevantes em memória, regravam. Suficiente para o cenário de hoje (um worker sequencial por job), mas não é uma garantia forte contra concorrência multi-writer no mesmo `idempotencyKey`.

## Pacotes relacionados

- `Dietcode.Core.Jobs.Interfaces`: define `IJobQueue`, `IAsyncJobStoreGeneric` e os modelos de domínio que este pacote implementa.
- `Dietcode.Core.Jobs`: implementação de referência (`JobAsyncService`, `JobWorkerGeneric`, `GenericJob`, `GenericJobHandler`) que consome `IJobQueue`/`IAsyncJobStoreGeneric` — funciona com esta implementação Redis sem nenhuma mudança de código, só trocando o registro no DI.

## Licença

MIT
