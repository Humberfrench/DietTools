# Dietcode.Core.Jobs

Implementação de referência para processamento assíncrono de jobs em background, sobre os contratos definidos em `Dietcode.Core.Jobs.Interfaces`. Fornece o serviço de aplicação que inicia jobs e consulta status/resultado, o job/handler genéricos e um `BackgroundService` que consome a fila e despacha o processamento.

## Instalação

```bash
dotnet add package Dietcode.Core.Jobs --version 10.1.0
```

## Funcionalidades

- `JobAsyncService<TRequest, TResult>`: implementa `IJobAsyncService<TRequest, TResult>` — inicia um job (`StartAsync`), consulta status (`GetStatusAsync`) e obtém o resultado desserializado (`GetResultAsync`). Retorna sempre `MethodResult` (de `Dietcode.Api.Core.Results`).
- `GenericJob`: implementação de `IJob` que carrega apenas a `IdempotencyKey` (mais `JobId`, igual à chave); é o tipo efetivamente enfileirado/desenfileirado pelo worker.
- `GenericJobHandler`: implementa `IJobHandler<GenericJob>` — busca o estado no store, despacha para o handler de negócio via `IHandlerDispatcher` usando a `HandlerKey` salva no job, e persiste o resultado (`SetCompletedAsync`) ou a falha (`SetFailedAsync`).
- `JobWorkerGeneric`: `BackgroundService` que fica consumindo `IJobQueue.DequeueAsync` em loop, cria um escopo de DI por job e delega a um `IJobHandler<GenericJob>` resolvido do container. Jobs de tipo desconhecido (diferente de `GenericJob`) apenas geram um log de aviso.

Este pacote depende das abstrações de `Dietcode.Core.Jobs.Interfaces`: para funcionar, a aplicação precisa registrar no DI implementações próprias de `IJobQueue`, `IAsyncJobStoreGeneric` e `IHandlerDispatcher`.

## Fluxo de uso

Registro no `Program.cs`:

```csharp
using Dietcode.Core.Jobs;
using Dietcode.Core.Jobs.Interfaces;

builder.Services.AddScoped(typeof(IJobAsyncService<,>), typeof(JobAsyncService<,>));
builder.Services.AddScoped<IJobHandler<GenericJob>, GenericJobHandler>();
builder.Services.AddHostedService<JobWorkerGeneric>();

// Implementações próprias da aplicação:
builder.Services.AddSingleton<IJobQueue, MinhaFilaDeJobs>();
builder.Services.AddSingleton<IAsyncJobStoreGeneric, MeuJobStore>();
builder.Services.AddSingleton<IHandlerDispatcher, MeuHandlerDispatcher>();
```

Iniciando um job assíncrono e acompanhando o resultado:

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
