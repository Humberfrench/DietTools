# Dietcode.Core.Jobs

Implementação de referência para processamento assíncrono de jobs em background, sobre os contratos definidos em `Dietcode.Core.Jobs.Interfaces`. Fornece o serviço de aplicação que inicia jobs e consulta status/resultado, o job/handler genéricos, um `BackgroundService` que consome a fila e despacha o processamento — e dois providers prontos de infraestrutura (`IJobQueue`/`IAsyncJobStoreGeneric`): **em memória** (padrão) e **Redis**.

O provider é escolhido num único lugar — `AddDietcodeJobs(primario, ...)` — igual ao padrão usado em `Dietcode.Core.Cep` (`AddDietcodeCep(primario, ...)`): você escolhe para onde vai, o pacote registra tudo. A diferença para o CEP é que **não há contingência automática entre os providers de Jobs**: fila e store são *stateful* (um job enfileirado em memória não existe no Redis e vice-versa), então alternar provider em runtime perderia jobs — a escolha é explícita e fixa por processo.

## Instalação

```bash
dotnet add package Dietcode.Core.Jobs --version 10.2.0
```

## Funcionalidades

- `JobAsyncService<TRequest, TResult>`: implementa `IJobAsyncService<TRequest, TResult>` — inicia um job (`StartAsync`), consulta status (`GetStatusAsync`) e obtém o resultado desserializado (`GetResultAsync`). Retorna sempre `MethodResult` (de `Dietcode.Api.Core.Results`).
- `GenericJob`: implementação de `IJob` que carrega apenas a `IdempotencyKey` (mais `JobId`, igual à chave); é o tipo efetivamente enfileirado/desenfileirado pelo worker.
- `GenericJobHandler`: implementa `IJobHandler<GenericJob>` — busca o estado no store, despacha para o handler de negócio via `IHandlerDispatcher` usando a `HandlerKey` salva no job, e persiste o resultado (`SetCompletedAsync`) ou a falha (`SetFailedAsync`).
- `JobWorkerGeneric`: `BackgroundService` que fica consumindo `IJobQueue.DequeueAsync` em loop, cria um escopo de DI por job e delega a um `IJobHandler<GenericJob>` resolvido do container. Jobs de tipo desconhecido (diferente de `GenericJob`) apenas geram um log de aviso.
- `JobsProviderPrimario`: `InMemory` ou `Redis` — o provider de infraestrutura escolhido em `AddDietcodeJobs(...)`.
- `InMemoryJobQueue`/`InMemoryJobStore` (namespace `Dietcode.Core.Jobs.Memory`): fila via `Channel<IJob>` e store via `ConcurrentDictionary`. Não sobrevivem a restart nem são compartilhados entre instâncias — é o comportamento de referência que qualquer outro provider (Redis incluso) reproduz.
- `RedisJobQueue`/`RedisJobStore` (namespace `Dietcode.Core.Jobs.Redis`): mesma coisa via Redis — sobrevive a restart, compartilhável entre instâncias. Ver seção "Provider Redis" abaixo.

A aplicação ainda precisa registrar seu próprio `IHandlerDispatcher` (é inerentemente específico do negócio — quem resolve e executa o handler pela `HandlerKey`).

## Fluxo de uso

### Registro — em memória (padrão, para dev/testes ou uma única instância)

```csharp
using Dietcode.Core.Jobs;
using Dietcode.Core.Jobs.Extensions;

builder.Services.AddDietcodeJobs(JobsProviderPrimario.InMemory);

builder.Services.AddSingleton<IHandlerDispatcher, MeuHandlerDispatcher>();
```

### Registro — Redis (sobrevive a restart, compartilhável entre instâncias)

```csharp
using Dietcode.Core.Jobs;
using Dietcode.Core.Jobs.Extensions;

builder.Services.AddDietcodeJobs(JobsProviderPrimario.Redis, options =>
{
    options.ConnectionString = "localhost:6379,syncTimeout=15000,asyncTimeout=15000";
});

builder.Services.AddSingleton<IHandlerDispatcher, MeuHandlerDispatcher>();
```

Ou lendo de `appsettings.json` via `IConfiguration`:

```json
{
  "RedisJobs": {
    "ConnectionString": "localhost:6379,syncTimeout=15000,asyncTimeout=15000",
    "QueueKey": "dietcode:jobs:queue",
    "StoreKeyPrefix": "dietcode:jobs:state:",
    "DequeuePollingSeconds": 5
  }
}
```

```csharp
builder.Services.AddDietcodeJobs(JobsProviderPrimario.Redis, builder.Configuration.GetSection("RedisJobs"));
```

`AddDietcodeJobs(...)` registra tudo de uma vez: `IJobAsyncService<,>` → `JobAsyncService<,>`, `IJobHandler<GenericJob>` → `GenericJobHandler`, `JobWorkerGeneric` como hosted service, e o par `IJobQueue`/`IAsyncJobStoreGeneric` do provider escolhido.

### Provider Redis — chaves de configuração (`RedisJobOptions`)

| Propriedade | Padrão | Descrição |
| --- | --- | --- |
| `ConnectionString` | `"localhost:6379"` | String de conexão do `StackExchange.Redis` (host, senha, TLS, timeouts etc.). |
| `Database` | `null` | Índice do banco Redis (`null` = padrão do multiplexer, banco 0). |
| `QueueKey` | `"dietcode:jobs:queue"` | Chave da lista Redis usada como fila. |
| `StoreKeyPrefix` | `"dietcode:jobs:state:"` | Prefixo das chaves de estado — cada job vira `{prefixo}{idempotencyKey}`. |
| `DequeuePollingSeconds` | `5` | Timeout (segundos) de cada `BRPOP`. Também é a latência máxima de cancelamento do `DequeueAsync` (ver abaixo). |
| `StateTimeToLive` | `null` | TTL do estado do job no Redis. `null` = sem expiração (igual ao provider em memória). |

**Trade-offs do provider Redis** (não existem no provider em memória, que usa `Channel`/`ConcurrentDictionary` nativos):

- **Fila FIFO simples, at-most-once**: `LPUSH`/`BRPOP` em loop. Sem redelivery, dead-letter ou consumer group — se o worker cair no meio do processamento de um job já retirado da lista, o job se perde (exigiria Redis Streams, fora do escopo aqui).
- **Cancelamento com latência limitada a `DequeuePollingSeconds`**: o `StackExchange.Redis` não cancela um `BRPOP` em voo, então o `CancellationToken` só é observado entre uma tentativa e a próxima.
- **`syncTimeout`/`asyncTimeout` da `ConnectionString` precisa ficar acima de `DequeuePollingSeconds`**: o `StackExchange.Redis` tem seu próprio timeout de cliente (padrão ~5s) e lança `RedisTimeoutException` se o servidor não responder dentro dele — independente do timeout do `BRPOP`. Configure algo como `"localhost:6379,syncTimeout=15000,asyncTimeout=15000"` para `DequeuePollingSeconds = 5`.
- **Store faz *read-modify-write*, não é atômico**: `SetCompletedAsync`/`SetFailedAsync` leem o JSON do estado, alteram os campos em memória, regravam. Suficiente para o cenário atual (worker sequencial por job), mas não é uma garantia forte contra concorrência multi-writer no mesmo `idempotencyKey`.

### Registro manual (sem `AddDietcodeJobs`)

Se preferir montar as peças na mão, ou plugar um terceiro provider (Mongo, SQL etc.), registre diretamente os contratos de `Dietcode.Core.Jobs.Interfaces`:

```csharp
using Dietcode.Core.Jobs;
using Dietcode.Core.Jobs.Interfaces;

builder.Services.AddScoped(typeof(IJobAsyncService<,>), typeof(JobAsyncService<,>));
builder.Services.AddScoped<IJobHandler<GenericJob>, GenericJobHandler>();
builder.Services.AddHostedService<JobWorkerGeneric>();

// Implementação própria (ou InMemoryJobQueue/InMemoryJobStore/RedisJobQueue/RedisJobStore direto):
builder.Services.AddSingleton<IJobQueue, MinhaFilaDeJobs>();
builder.Services.AddSingleton<IAsyncJobStoreGeneric, MeuJobStore>();
builder.Services.AddSingleton<IHandlerDispatcher, MeuHandlerDispatcher>();
```

### Iniciando um job assíncrono e acompanhando o resultado

```csharp
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;

public sealed class RelatorioController
{
    private readonly IJobAsyncService<RelatorioInput, RelatorioOutput> _jobService;

    public RelatorioController(IJobAsyncService<RelatorioInput, RelatorioOutput> jobService)
    {
        _jobService = jobService;
    }

    public async Task<AsyncReturn> Iniciar(RelatorioInput input, CancellationToken ct)
    {
        var request = new AsyncStartRequest<RelatorioInput>("gerar-relatorio", input);
        var started = await _jobService.StartAsync(request, ct);

        return started.Content;
    }
}
```

Enquanto o job está em `Processing`, `GetResultAsync` retorna erro com `ResultStatusCode.Accepted` (202). Quando `Completed`, retorna `Ok` com o conteúdo desserializado; quando `Failed`, retorna `ResultStatusCode.InternalServerError` (500) com a mensagem de erro.

## Pacotes relacionados

- `Dietcode.Api.Core.Results`: `MethodResult`, `ResultStatusCode` e `AppServiceBase`, usados por `JobAsyncService`.
- `Dietcode.Core.Jobs.Interfaces`: define os contratos (`IJob`, `IJobQueue`, `IJobHandler<TJob>`, `IHandlerDispatcher`, `IAsyncJobStoreGeneric`, `IJobAsyncService<TRequest, TResult>`) e os modelos de domínio (`AsyncStartRequest<TRequest>`, `AsyncJobStateGeneric`, `AsyncReturn`, `JobStatus`) que este pacote implementa.

## Licença

MIT
