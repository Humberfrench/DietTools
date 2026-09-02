# Dietcode.Core.Jobs.Interfaces

Contratos e modelos de domínio para processamento assíncrono de jobs em background: interfaces de fila, store de estado, dispatcher de handlers e o modelo de requisição/retorno usado pelo serviço assíncrono. Este pacote não contém implementação — apenas os tipos que `Dietcode.Core.Jobs` implementa e que a aplicação consumidora precisa fornecer (fila, store etc.).

## Instalação

```bash
dotnet add package Dietcode.Core.Jobs.Interfaces --version 10.1.0
```

## Funcionalidades

- `IJob`: marcador para um trabalho enfileirável (`IdempotencyKey`, `JobName`).
- `IJobQueue`: fila assíncrona de jobs (`EnqueueAsync`, `DequeueAsync`).
- `IJobHandler<in TJob>`: contrato de handler para processar um `IJob` específico.
- `IHandlerDispatcher`: resolve e executa um handler de negócio a partir de uma `HandlerKey`, recebendo e devolvendo payload em JSON.
- `IAsyncJobStoreGeneric`: persistência do estado do job genérico (criar, consultar, marcar concluído/falho). Implementações típicas: em memória, Redis, Mongo, SQL.
- `IJobAsyncService<TRequest, TResult>`: contrato do serviço de aplicação que inicia jobs e consulta status/resultado, retornando `MethodResult` (de `Dietcode.Api.Core.Results`).
- `AsyncStartRequest<TRequest>`: `record` com `HandlerKey` (identifica o handler a executar) e `Payload` (dados de entrada). Está no namespace `Dietcode.Core.Jobs`, embora resida neste pacote.
- `AsyncJobStateGeneric`: estado persistido do job (chave de idempotência, `HandlerKey`, payload e resultado em JSON, status, datas de criação/conclusão, erro).
- `AsyncReturn`: retorno padronizado com `IdempotencyKey`, `Status` (`JobStatus`), `StatusText` e `StatusCode` (`ResultStatusCode`).
- `JobStatus`: enum de status do job — `NotFound`, `Processing`, `Completed`, `Failed`, `Unknown`.
- `GenericJob` (em `Dietcode.Core.Jobs.Interfaces.Domain`): implementação de `IJob` que carrega apenas a `IdempotencyKey`; o worker busca `HandlerKey`/payload no store.

## Contratos principais

```csharp
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;

public interface IJob
{
    string IdempotencyKey { get; }
    string JobName { get; }
}

public interface IJobQueue
{
    ValueTask EnqueueAsync(IJob job, CancellationToken ct);
    ValueTask<IJob> DequeueAsync(CancellationToken ct);
}

public interface IJobHandler<in TJob> where TJob : IJob
{
    Task HandleAsync(TJob job, CancellationToken ct);
}

public interface IAsyncJobStoreGeneric
{
    Task CreateAsync(AsyncJobStateGeneric job, CancellationToken ct);
    Task<AsyncJobStateGeneric?> GetAsync(string idempotencyKey, CancellationToken ct);
    Task SetCompletedAsync(string idempotencyKey, string resultJson, CancellationToken ct);
    Task SetFailedAsync(string idempotencyKey, string error, CancellationToken ct);
}

public interface IHandlerDispatcher
{
    Task<string> ExecuteAsync(string handlerKey, string payloadJson, CancellationToken ct);
}
```

## Serviço assíncrono

```csharp
using Dietcode.Api.Core.Results;
using Dietcode.Core.Jobs.Interfaces;
using Dietcode.Core.Jobs.Interfaces.Domain;

public interface IJobAsyncService<TRequest, TResult>
{
    Task<MethodResult<AsyncReturn>> StartAsync(AsyncStartRequest<TRequest> request, CancellationToken ct);
    Task<MethodResult<AsyncReturn>> GetStatusAsync(string idempotencyKey, CancellationToken ct);
    Task<MethodResult<TResult>> GetResultAsync(string idempotencyKey, CancellationToken ct);
}
```

Uso típico em um controller ou outro service:

```csharp
var request = new AsyncStartRequest<RelatorioInput>("gerar-relatorio", input);
MethodResult<AsyncReturn> started = await jobAsyncService.StartAsync(request, ct);

// Mais tarde, consultando o status:
MethodResult<AsyncReturn> status = await jobAsyncService.GetStatusAsync(started.Content.IdempotencyKey, ct);

// Quando concluído, obtendo o resultado desserializado:
MethodResult<RelatorioOutput> resultado = await jobAsyncService.GetResultAsync(started.Content.IdempotencyKey, ct);
```

## Pacotes relacionados

- `Dietcode.Api.Core.Results`: fornece `MethodResult` e `ResultStatusCode`, usados em `IJobAsyncService` e `AsyncReturn`.
- `Dietcode.Core.Jobs`: implementa estas interfaces — fornece `JobAsyncService<TRequest, TResult>`, `JobWorkerGeneric` (worker em background) e o handler/job genéricos. A aplicação consumidora ainda precisa implementar `IJobQueue`, `IAsyncJobStoreGeneric` e `IHandlerDispatcher` de acordo com sua infraestrutura (memória, Redis, banco de dados etc.).

## Licença

MIT
