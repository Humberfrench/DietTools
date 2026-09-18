# DietTools

Monorepositório com as bibliotecas e ferramentas internas da Dietcode: um conjunto de pacotes .NET reutilizáveis entre os produtos da empresa, cobrindo API REST (ASP.NET Core), validação e regras de domínio, acesso a dados, jobs em background, e-mail, busca de CEP, criptografia, QR Code e utilitários gerais — além das versões legadas equivalentes para projetos ainda em .NET Framework 4.8.

Cada projeto também tem seu próprio `README.md`, com mais detalhes e exemplos adicionais. Este documento traz, para cada um: o que é, o que faz, suas principais funcionalidades e um exemplo mínimo de uso.

## Como abrir

A solução principal é [`Dietcode.Api.Core.sln`](Dietcode.Api.Core.sln) — reúne a maior parte dos projetos, organizados nas pastas de solução descritas abaixo. `Tools` e `NugetServer` têm `.sln` próprios e não fazem parte dela.

## Índice

- [00 — API REST](#00--api-rest): [Dietcode.Api.Core](#dietcodeapicore) · [Dietcode.Api.Core.Results](#dietcodeapicoreresults)
- [01 — Bibliotecas (.NET moderno)](#01--bibliotecas-net-moderno): [Dietcode.Core.Lib](#dietcodecorelib) · [Dietcode.Core.Lib.Codes](#dietcodecorelibcodes) · [Dietcode.Core.DomainValidator](#dietcodecoredomainvalidator) · [Dietcode.Core.Domain.Rules](#dietcodecoredomainrules) · [Dietcode.Core.Jobs](#dietcodecorejobs) · [Dietcode.Core.Jobs.Interfaces](#dietcodecorejobsinterfaces) · [Dietcode.Core.Email](#dietcodecoreemail) · [Dietcode.Core.Security](#dietcodecoresecurity) · [Dietcode.Core.Cep](#dietcodecorecep)
- [02 — Acesso a dados (.NET moderno)](#02--acesso-a-dados-net-moderno): [Dietcode.Database](#dietcodedatabase) · [Dietcode.Database.Domain](#dietcodedatabasedomain) · [Dietcode.Database.Orm](#dietcodedatabaseorm) · [Dietcode.Database.Classic](#dietcodedatabaseclassic)
- [12 — Acesso a dados (legado, .NET Framework 4.8)](#12--acesso-a-dados-legado-net-framework-48): [Dietcode.Database.Net.Domain](#dietcodedatabasenetdomain) · [Dietcode.Database.Net.Orm](#dietcodedatabasenetorm)
- [11 — Bibliotecas (legado, .NET Framework 4.8)](#11--bibliotecas-legado-net-framework-48): [Dietcode.Classic.Lib](#dietcodeclassiclib) · [Dietcode.Classic.Domain.Rules](#dietcodeclassicdomainrules) · [Dietcode.Classic.DomainValidator](#dietcodeclassicdomainvalidator)
- [99 — Ferramentas e infraestrutura](#99--ferramentas-e-infraestrutura): [Tools](#tools) · [NugetServer](#nugetserver)

---

## 00 — API REST

Camada de infraestrutura HTTP para APIs ASP.NET Core: converte o resultado de aplicação em resposta HTTP.

### Dietcode.Api.Core

**O que é / o que faz:** biblioteca de apoio para APIs ASP.NET Core. Centraliza recursos comuns de infraestrutura HTTP — conversão de resultados de aplicação em respostas HTTP, logging de requests/responses e rate limiting simples por endpoint. Depende de `Dietcode.Api.Core.Results` para o modelo padronizado de retorno.

**Funcionalidades:**
- `ApiControllerBase`: controller base que converte `MethodResult`/`MethodResult<T>` em `IActionResult`.
- Conversão automática de erros para `ProblemDetails` (um erro) ou `ValidationProblemDetails` (vários erros).
- Hook `BeforeReturn` para auditoria, logs ou enriquecimento antes da resposta.
- Rate limit por atributo (`[RateLimit(limit, seconds)]`), baseado em IP e endpoint.
- Middleware de logging estruturado em JSON Lines (`.jsonl`), com mascaramento de dados sensíveis.
- Middleware legado de logging simples em texto.

**Exemplo:**
```csharp
using Dietcode.Api.Core;
using Dietcode.Api.Core.Results;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    private readonly UserService _service;

    public UsersController(UserService service) => _service = service;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        MethodResult<UserDto> result = await _service.GetAsync(id, ct);
        return Completed<UserDto>(result);
    }
}
```

Mais detalhes: logging estruturado, rate limiting e o comportamento de `Created` em [Dietcode.Api.Core/README.md](Dietcode.Api.Core/README.md).

### Dietcode.Api.Core.Results

**O que é / o que faz:** modelo padronizado de resultado (`MethodResult`) para aplicações .NET e APIs ASP.NET Core, usado pela camada de aplicação sem depender de `IActionResult`, `ControllerBase` ou detalhes de HTTP. É a base consumida por `Dietcode.Api.Core`.

**Funcionalidades:**
- `MethodResult` / `MethodResult<TContent>`: resultado base e com conteúdo.
- Resultados de sucesso: `OkResult`, `CreatedResult`, `AcceptedResult`, `NoContentResult`.
- Resultados de erro: `BadRequestResult`, `NotFoundResult`, `ConflictResult`, `UnauthorizedResult`, `ForbiddenResult`, `UnprocessableEntityResult`, `TooManyRequestsResult`, entre outros.
- `AppServiceBase`: classe base com métodos de fábrica (`Ok`, `Created`, `BadRequest`, `NotFound`...) para a camada de serviço.
- `Propagate<TContent>`: repropaga o erro de um `MethodResult` interno preservando `Status`/`Errors`, sem saber o tipo concreto.
- `ErrorBuilder`: cria erros a partir de enums e `ResourceManager`.

**Exemplo:**
```csharp
using Dietcode.Api.Core.Results;

public sealed class UserService : AppServiceBase
{
    public MethodResult<UserDto> Get(int id)
    {
        if (id <= 0)
            return BadRequest<UserDto>("Id invalido.", new UserDto());

        return Ok(new UserDto { Id = id, Name = "Maria" });
    }
}
```

Mais detalhes: todos os `ResultStatusCode`, `Propagate` e `ErrorBuilder` em [Dietcode.Api.Core.Results/README.md](Dietcode.Api.Core.Results/README.md).

## 01 — Bibliotecas (.NET moderno)

### Dietcode.Core.Lib

**O que é / o que faz:** biblioteca de utilitários gerais para aplicações .NET — extensões, formatadores, validadores, helpers JSON, mascaramento de dados, paginação, localização, análise de senha e chamadas REST simples.

**Funcionalidades:**
- Extensões de string, número, data (incluindo `DateOnly`), JSON e enum.
- Validação/formatação de CPF e CNPJ (`Validacao`, `ToCpf()`, `ToCnpj()`).
- Mascaramento de dados sensíveis (`SensitiveDataMasker`).
- `JsonOptionsFactory` com opções padrão para `System.Text.Json`.
- Análise de força de senha (`AnalyzePassword()`).
- Paginação, localização simples por dicionário.
- `HttpService`: helper REST estático (GET/POST/PUT/PATCH/DELETE) com retorno padronizado (`ApiResult<T>`).

**Exemplo:**
```csharp
using Dietcode.Core.Lib;
using Dietcode.Core.Lib.Rest;

var cpfFormatado = "12345678909".ToCpf();
var cpfValido = Validacao.IsCpf("12345678909");

ApiResult<UserResponse> result = await HttpService.Get<UserResponse>(
    url: "https://api.exemplo.com/users/1",
    enumApiRest: EnumApiRest.Bearer,
    token: accessToken);

if (result.IsSuccess)
{
    var user = result.Data;
}
```

Mais detalhes: extensões de data, senhas, JSON e localização em [Dietcode.Core.Lib/README.md](Dietcode.Core.Lib/README.md).

### Dietcode.Core.Lib.Codes

**O que é / o que faz:** biblioteca de geração de QR Code para .NET 10. Combina um motor de codificação completo (portado do projeto [QRCoder](https://github.com/codebude/QRCoder), MIT) com renderização própria — sem `System.Drawing`, portanto multiplataforma — e uma API simplificada (`QrEncoder`). Não gera pacote NuGet; é consumida via referência de projeto.

**Funcionalidades:**
- Motor `QRCodeGenerator` (versões 1–40 e Micro QR, detecção automática de modo, níveis de correção L/M/Q/H).
- `QrEncoder`: API simplificada para gerar `QRCodeData`, PNG (`byte[]`) ou PNG em Base64.
- Renderizadores próprios: `PngByteQRCode`, `Base64QRCode` (cores customizáveis).
- Mais de vinte geradores de payload (`PayloadGenerator`): Wi-Fi, vCard, e-mail, SMS, criptomoedas, pagamentos bancários europeus etc.
- Serialização binária compacta de `QRCodeData` (`.qrr`), com compressão `Deflate`/`GZip`.

**Exemplo:**
```csharp
using Dietcode.Core.Lib.Codes;

using var encoder = new QrEncoder();

string base64Png = encoder.EncodeToBase64Png(
    text: "https://www.dietcode.com.br",
    level: QrErrorCorrectionLevel.M,
    pixelsPerModule: 10);
```

Mais detalhes: lista completa de payloads e persistência de `QRCodeData` em [Dietcode.Core.Lib.Codes/README.md](Dietcode.Core.Lib.Codes/README.md).

### Dietcode.Core.DomainValidator

**O que é / o que faz:** representa resultados de validação de domínio (net10.0) — agrega erros, mensagens informativas, status HTTP e metadados de forma padronizada entre domínio, aplicação e API.

**Funcionalidades:**
- `ValidationResultBase` / `ValidationResult<T>`: erros, mensagens, flags `Valid`/`Invalid`, `HttpStatusCode`.
- `ValidationResult` não genérico, com `TryReturnAs<T>()`.
- `Converter<U>()`: converte `ValidationResult<T>` em `ValidationResult<U>` via JSON.
- Renderização de erros/mensagens como texto ou HTML.

**Exemplo:**
```csharp
using Dietcode.Core.DomainValidator;

public ValidationResult<UsuarioDto> Cadastrar(UsuarioDto usuario)
{
    var result = new ValidationResult<UsuarioDto>();

    if (string.IsNullOrWhiteSpace(usuario.Nome))
        result.AddError("Nome é obrigatório.");

    if (result.Invalid)
        return result;

    result.Retorno = usuario;
    return result;
}
```

Mais detalhes: agregação de erros e mensagens informativas em [Dietcode.Core.DomainValidator/README.md](Dietcode.Core.DomainValidator/README.md).

### Dietcode.Core.Domain.Rules

**O que é / o que faz:** composição de regras de validação de domínio (net10.0) baseada no padrão *Specification* — regras de negócio reutilizáveis, fortemente tipadas, expressas por lambda sobre as propriedades da entidade.

**Funcionalidades:**
- `ISpecification<T>`, `IRule<TEntity>`, `Rule<TEntity>`, `Validator<TEntity>` (padrão *Strategy*).
- `ValidatorRules`: resultado com `Errors`, `Valid`/`Invalid`, renderização texto/HTML.
- Especificações prontas: string preenchida, número > 0 / >= 0, e-mail válido, mês/dia válido, "requisito mínimo preenchido" (ao menos um de vários campos).

**Exemplo:**
```csharp
using Dietcode.Core.Domain.Rules;
using Dietcode.Core.Domain.Rules.Specifications;

public class UsuarioValidator : Validator<Usuario>
{
    public UsuarioValidator()
    {
        AdicionarRegra("EmailValido",
            new Rule<Usuario>(new PropriedadeEmailValido<Usuario>(u => u.Email),
                "E-mail inválido."));
    }
}

ValidatorRules resultado = new UsuarioValidator().Validar(usuario);
```

Mais detalhes: todas as especificações prontas em [Dietcode.Core.Domain.Rules/README.md](Dietcode.Core.Domain.Rules/README.md).

### Dietcode.Core.Jobs

**O que é / o que faz:** implementação de referência para processamento assíncrono de jobs em background, sobre os contratos de `Dietcode.Core.Jobs.Interfaces`. Fornece o serviço que inicia jobs e consulta status/resultado, o job/handler genéricos e um `BackgroundService` que consome a fila.

**Funcionalidades:**
- `JobAsyncService<TRequest, TResult>`: `StartAsync`, `GetStatusAsync`, `GetResultAsync` (retorna `MethodResult`).
- `GenericJob` / `GenericJobHandler`: job enfileirado e handler que despacha para a lógica de negócio via `IHandlerDispatcher`.
- `JobWorkerGeneric`: `BackgroundService` que consome `IJobQueue` em loop.
- Requer que a aplicação registre `IJobQueue`, `IAsyncJobStoreGeneric` e `IHandlerDispatcher` (de `Dietcode.Core.Jobs.Interfaces`).

**Exemplo:**
```csharp
using Dietcode.Core.Jobs;
using Dietcode.Core.Jobs.Interfaces;

builder.Services.AddScoped(typeof(IJobAsyncService<,>), typeof(JobAsyncService<,>));
builder.Services.AddHostedService<JobWorkerGeneric>();

var request = new AsyncStartRequest<RelatorioInput>("gerar-relatorio", input);
MethodResult<AsyncReturn> started = await jobService.StartAsync(request, ct);
```

Mais detalhes: fluxo completo de status (`Processing`/`Completed`/`Failed`) em [Dietcode.Core.Jobs/README.md](Dietcode.Core.Jobs/README.md).

### Dietcode.Core.Jobs.Interfaces

**O que é / o que faz:** contratos e modelos de domínio para jobs em background — fila, store de estado, dispatcher de handlers e request/retorno usados pelo serviço assíncrono. Não contém implementação; é o que `Dietcode.Core.Jobs` implementa e a aplicação consumidora precisa fornecer (fila, store etc.).

**Funcionalidades:**
- `IJob`, `IJobQueue` (`EnqueueAsync`/`DequeueAsync`), `IJobHandler<TJob>`, `IHandlerDispatcher`.
- `IAsyncJobStoreGeneric`: persistência do estado do job (memória, Redis, Mongo, SQL etc.).
- `IJobAsyncService<TRequest, TResult>`, `AsyncStartRequest<TRequest>`, `AsyncReturn`, `JobStatus`.

**Exemplo:**
```csharp
public interface IJobQueue
{
    ValueTask EnqueueAsync(IJob job, CancellationToken ct);
    ValueTask<IJob> DequeueAsync(CancellationToken ct);
}
```

Mais detalhes: todos os contratos em [Dietcode.Core.Jobs.Interfaces/README.md](Dietcode.Core.Jobs.Interfaces/README.md).

### Dietcode.Core.Email

**O que é / o que faz:** biblioteca para envio de e-mails via SMTP autenticado, usando MailKit. Foi separada do `Dietcode.Core.Lib` para não empurrar dependências de e-mail em projetos que não precisam.

**Funcionalidades:**
- `IEmailSender` / `SmtpEmailSender`: envio assíncrono via SMTP (STARTTLS ou SSL).
- `EmailMessage`: destinatários (`To`/`Cc`/`Bcc`/`ReplyTo`), assunto, corpo texto/HTML, anexos (inclusive inline) e headers.
- `EmailSendResult`: `IsSuccess`/`MessageId`/`Error`, sem lançar exceção para falhas de validação.
- `AddDietcodeSmtpEmail(...)`: registro via DI, por `IConfiguration` ou lambda.

**Exemplo:**
```csharp
using Dietcode.Core.Email.Abstractions;
using Dietcode.Core.Email.Models;

builder.Services.AddDietcodeSmtpEmail(builder.Configuration.GetSection("SmtpEmail"));

// Em outra classe, injetando IEmailSender:
var result = await emailSender.SendAsync(new EmailMessage
{
    To = ["cliente@exemplo.com"],
    Subject = "Bem-vindo",
    HtmlBody = "<strong>Olá!</strong>"
});
```

Mais detalhes: anexos, TLS/SSL e validações em [Dietcode.Core.Email/README.md](Dietcode.Core.Email/README.md).

### Dietcode.Core.Security

**O que é / o que faz:** criptografia simétrica AES para aplicações .NET — AES-GCM autenticado (atual) e leitura de compatibilidade com o formato legado AES-ECB. Substitui a criptografia que antes vivia em `Dietcode.Core.Lib`.

**Funcionalidades:**
- `AES.Encrypt`/`AES.Decrypt`: AES-GCM com derivação de chave PBKDF2, nonce aleatório e tag de autenticação; prefixo `v2:`.
- `Decrypt` cai automaticamente para o formato ECB legado quando o payload não tem o prefixo `v2:`.
- `V1.AES` (`[Obsolete]`): AES-ECB legado, mantido só para ler dados antigos.

**Exemplo:**
```csharp
using Dietcode.Core.Security;

string? cifrado = AES.Encrypt("dado sensivel", "minha-chave");
string? texto = AES.Decrypt(cifrado, "minha-chave");
```

Mais detalhes: detalhes de implementação e o formato legado em [Dietcode.Core.Security/README.md](Dietcode.Core.Security/README.md).

### Dietcode.Core.Cep

**O que é / o que faz:** biblioteca para consulta de endereço a partir de um CEP, com o provedor plugável via injeção de dependência. Hoje o único provedor é o [ViaCEP](https://viacep.com.br/) (v1); trocar ou adicionar um novo provedor no futuro não muda a assinatura de `ICepProvider` nem o código de quem já consome a biblioteca.

**Funcionalidades:**
- `ICepProvider.GetAddressAsync(cep)`: contrato estável, independente do provedor.
- `ViaCepProvider`: implementação v1 (ViaCEP), com `HttpClient` tipado via `IHttpClientFactory`.
- `CepLookupResult` (`Success`/`Failure`): nunca lança exceção para CEP inválido ou não encontrado.
- `AddDietcodeViaCep(...)`: registro via DI, com `BaseUrl`/`TimeoutSeconds` configuráveis.

**Exemplo:**
```csharp
using Dietcode.Core.Cep.Abstractions;
using Dietcode.Core.Cep.Extensions;

builder.Services.AddDietcodeViaCep();

// Em outra classe, injetando ICepProvider:
public sealed class EnderecoService(ICepProvider cepProvider)
{
    public async Task<string> DescreverAsync(string cep, CancellationToken ct)
    {
        var resultado = await cepProvider.GetAddressAsync(cep, ct);

        return resultado.IsSuccess
            ? $"{resultado.Address!.Logradouro} - {resultado.Address.Localidade}/{resultado.Address.Uf}"
            : $"Erro: {resultado.Error}";
    }
}
```

Mais detalhes: como plugar um novo provedor (ex.: v2) sem quebrar consumidores em [Dietcode.Core.Cep/README.md](Dietcode.Core.Cep/README.md).

## 02 — Acesso a dados (.NET moderno)

### Dietcode.Database

**O que é / o que faz:** infraestrutura leve e assíncrona de acesso a dados com Dapper/Dapper.Contrib, com suporte a múltiplos bancos (SQL Server, PostgreSQL, MySQL, Oracle). Independente da família `Dietcode.Database.Domain`/`Orm` (não usa Entity Framework).

**Funcionalidades:**
- `DapperRepository<T>` genérico assíncrono, via `IRepository<T>`/`IReadRepository<T>`/`IWriteRepository<T>`.
- `IConnectionFactory` com fábricas para cada banco suportado.
- `DapperUnitOfWork`: execução dentro de transação, com commit/rollback automático.
- Atributos de mapeamento (`KeyId`, `ExplicitKeyId`, `ComputedCol`, `WriteCol`, `TableName`) sobre o Dapper.Contrib.
- Logging estruturado (`IRepositoryLogger`/`JsonRepositoryLogger`) com mascaramento de dados sensíveis.

**Exemplo:**
```csharp
builder.Services.AddDietcodeSqlServer(builder.Configuration.GetConnectionString("Default"));

public class UserService(IRepository<User> repository)
{
    public Task<User?> GetAsync(int id, CancellationToken ct) =>
        repository.GetByIdAsync(id, ct);
}
```

Mais detalhes: Unit of Work, atributos de mapeamento e logging em [Dietcode.Database/README.md](Dietcode.Database/README.md).

### Dietcode.Database.Domain

**O que é / o que faz:** contratos (interfaces) de repositório, unit of work e contexto ambiente para a família de acesso a dados baseada em Entity Framework Core (net10.0). Não contém implementação — define o "shape" que `Dietcode.Database.Orm` implementa. Equivalente moderno de `Dietcode.Database.Net.Domain`.

**Funcionalidades:**
- `IBaseRepository<Table, Tipo>`: CRUD assíncrono, paginação, pesquisa por predicado (com `Include`), contagem, existência, inserção em lote.
- `IMyUnitOfWork<T>` / `IMyContextManager<ContextT>`.
- `IAmbientContextStore`: contexto corrente agnóstico de host (web ou worker).
- `ICompositeKey`: suporte a chave composta.

**Exemplo:**
```csharp
public interface IBaseRepository<Table, Tipo> : IDisposable where Table : class, new()
{
    Task<Table?> ObterPorId(Tipo id, bool asTracking = false, CancellationToken ct = default);
    Task<bool> Adicionar(Table obj, CancellationToken ct = default);
    Task<ValidationResult<Table>> Commit(CancellationToken ct = default);
}
```

Mais detalhes: `IAmbientContextStore` e chave composta em [Dietcode.Database.Domain/README.md](Dietcode.Database.Domain/README.md).

### Dietcode.Database.Orm

**O que é / o que faz:** implementação com Entity Framework Core (SQL Server) dos contratos de `Dietcode.Database.Domain`: repositório genérico, unit of work, gerenciamento de contexto e logging estruturado com Serilog.

**Funcionalidades:**
- `Builder.BuilderStart(services)`: registra `IBaseRepository<,>`, `IMyContextManager<>`, `IMyUnitOfWork<>` (Scoped) e `IAmbientContextStore` (Singleton).
- `ThisDatabase<T>`: `DbContext` que lê `DbContextConnString` de `appsettings.json`.
- `BaseRepository<Table, Tipo>`: chave simples ou composta, paginação, `Include`, consultas Dapper na mesma connection string.
- `AmbientContextStore` baseado em `AsyncLocal`, funciona tanto em web quanto em workers.

**Exemplo:**
```csharp
using Dietcode.Database.Orm;

Builder.BuilderStart(builder.Services);

public class UserService(IBaseRepository<User, int> repository)
{
    public Task<User?> GetAsync(int id, CancellationToken ct) =>
        repository.ObterPorId(id, ct: ct);
}
```

Mais detalhes: consultas Dapper dentro do repositório e configuração da connection string em [Dietcode.Database.Orm/README.md](Dietcode.Database.Orm/README.md).

### Dietcode.Database.Classic

**O que é / o que faz:** wrapper simplificado sobre Entity Framework Core (SQL Server), com um `DbContext` por entidade e repositório genérico com CRUD básico — sem os contratos de `Dietcode.Database.Domain` nem Unit of Work explícito. Alternativa mais enxuta para cenários simples (apesar do nome, não é o legado .NET Framework).

**Funcionalidades:**
- `Database.Configure(connectionString, version)`: configuração global (retry automático, `SQLVersion`).
- `Database<T>`: `DbContext` genérico com uma entidade.
- `BaseRepository<T>`: `Adicionar`/`Atualizar`/`Remover`/`Get`/`LoadAll`, `Query(sql)` via Dapper, transação manual.

**Exemplo:**
```csharp
using Dietcode.Database.Classic;

Database.Configure(connectionString, Database.SQLVersion.SQL2019);

var repository = new BaseRepository<User> { SaveAuto = true };
var novoId = await repository.Adicionar(new User { Email = "user@exemplo.com" });
```

Mais detalhes: transação manual e consulta Dapper em [Dietcode.Database.Classic/README.md](Dietcode.Database.Classic/README.md).

## 12 — Acesso a dados (legado, .NET Framework 4.8)

### Dietcode.Database.Net.Domain

**O que é / o que faz:** contratos equivalentes a `Dietcode.Database.Domain`, para aplicações ASP.NET clássicas (Web Forms/MVC sobre `System.Web`). Não é publicado como pacote NuGet — referenciado via `ProjectReference`.

**Funcionalidades:**
- `IBaseRepository<TEntity>`: repositório assíncrono, sem chave composta e sem `CancellationToken`, chave sempre `int`.
- `IMyUnitOfWork<T>` síncrono e `IMyContextManager<ContextT>`.

**Exemplo:**
```csharp
public interface IBaseRepository<TEntity> : IDisposable
{
    Task<TEntity> ObterPorId(int id);
    Task<IEnumerable<TEntity>> Pesquisar(Expression<Func<TEntity, bool>> predicate);
}
```

Mais detalhes: diferenças em relação à versão moderna em [Dietcode.Database.Net.Domain/README.md](Dietcode.Database.Net.Domain/README.md).

### Dietcode.Database.Net.Orm

**O que é / o que faz:** implementação legada, com Entity Framework 6 e Dapper, dos contratos de `Dietcode.Database.Net.Domain`, para aplicações ASP.NET clássicas (`HttpContext.Current`). Equivalente legado de `Dietcode.Database.Orm`.

**Funcionalidades:**
- `ThisDatabase`/`ThisDatabase<Table>`: `DbContext` (EF6) com lazy loading desativado.
- `MyContextManager<T>`: guarda o contexto em `HttpContext.Current.Items` (um por requisição).
- `BaseRepository<TEntity>` e `MyUnitOfWork<T>` (síncrono), com `Connection` pronta para Dapper.

**Exemplo:**
```csharp
var contextManager = new MyContextManager<ThisDatabase<User>>();
var repository = new UserRepository(contextManager);

var usuario = await repository.ObterPorId(1);
```

Mais detalhes: configuração de connection string via `web.config` em [Dietcode.Database.Net.Orm/README.md](Dietcode.Database.Net.Orm/README.md).

## 11 — Bibliotecas (legado, .NET Framework 4.8)

### Dietcode.Classic.Lib

**O que é / o que faz:** versão de `Dietcode.Core.Lib` para .NET Framework 4.8 — extensões, validadores de documentos brasileiros, criptografia, mascaramento, senha, REST e componentes para ASP.NET MVC clássico. Usa `Newtonsoft.Json` em vez de `System.Text.Json` e não tem `DateOnly` nem localização.

**Funcionalidades:**
- Validação/formatação de CPF, CNPJ, RG, CNH, Renavam e cartão de crédito (Luhn + bandeira).
- Validação de telefone fixo/celular no padrão brasileiro.
- `HttpService` (REST), `AES` (ECB), `AppSettings` (via `ConfigurationManager`).
- Componentes de view ASP.NET MVC: `Aviso`, `BreadCrumb`, `AsIsBundleOrderer`.

**Exemplo:**
```csharp
using Dietcode.Classic.Lib;

var cpfFormatado = "12345678909".ToCpf();
var bandeira = CreditCardValidator.ValidaBandeira("4111111111111111"); // "VISA"
```

Mais detalhes: cálculo financeiro, geração de senhas aleatórias e utilidades diversas em [Dietcode.Classic.Lib/README.md](Dietcode.Classic.Lib/README.md).

### Dietcode.Classic.Domain.Rules

**O que é / o que faz:** versão legada (.NET Framework 4.8) de `Dietcode.Core.Domain.Rules`, com API idêntica — composição de regras via padrão *Specification*.

**Funcionalidades:** mesmas de `Dietcode.Core.Domain.Rules` — `ISpecification<T>`, `Rule<TEntity>`, `Validator<TEntity>` e as especificações prontas (string preenchida, e-mail válido, etc.).

**Exemplo:**
```csharp
using Dietcode.Classic.Domain.Rules;
using Dietcode.Classic.Domain.Rules.Specifications;

AdicionarRegra("EmailValido",
    new Rule<Usuario>(new PropriedadeEmailValido<Usuario>(u => u.Email), "E-mail inválido."));
```

Mais detalhes em [Dietcode.Classic.Domain.Rules/README.md](Dietcode.Classic.Domain.Rules/README.md).

### Dietcode.Classic.DomainValidator

**O que é / o que faz:** versão legada (.NET Framework 4.8) de `Dietcode.Core.DomainValidator`, com API equivalente. Mantém `Mensagem`/`CodigoMensagem` marcadas `[Obsolete(error: true)]` para sinalizar a migração para `Mensagens`/`AddMensagem` — a versão Core já as removeu.

**Funcionalidades:** mesmas de `Dietcode.Core.DomainValidator` — `ValidationResult<T>`, `AddError`, mensagens informativas, renderização texto/HTML, `Converter<U>()`.

**Exemplo:**
```csharp
using Dietcode.Classic.DomainValidator;

var result = new ValidationResult<UsuarioDto>();
result.AddError("E-mail é obrigatório.", codigo: 101);
```

Mais detalhes em [Dietcode.Classic.DomainValidator/README.md](Dietcode.Classic.DomainValidator/README.md).

## 99 — Ferramentas e infraestrutura

Não são pacotes publicados para consumo externo; são apps internos de apoio ao desenvolvimento.

### Tools

**O que é / o que faz:** projeto de biblioteca (`netstandard2.0`) atualmente vazio, com apenas uma classe placeholder (`Class1`) sem membros. Parece ser um esqueleto para uma futura biblioteca de utilitários compartilhados, ainda não desenvolvida.

**Funcionalidades:** nenhuma até o momento.

```bash
dotnet build Tools/Tools.csproj
```

Mais detalhes em [Tools/README.md](Tools/README.md).

### NugetServer

**O que é / o que faz:** servidor NuGet privado local, montado com o pacote `NuGet.Server` sobre um projeto ASP.NET (Web Forms + Web API/OData, .NET Framework 4.8). Não é biblioteca nem app de console: é uma aplicação web (IIS/IIS Express) para hospedar e servir localmente os `.nupkg` das bibliotecas Dietcode durante o desenvolvimento.

**Funcionalidades:**
- Rotas OData do feed (`nuget/...`) e limpeza de cache (`nuget/clear-cache`).
- `Packages/`: pasta onde os `.nupkg` publicados ficam armazenados e são servidos.
- Autenticação por `apiKey` configurável em `Web.config` (opcional, para uso local).

**Exemplo (publicar um pacote no feed):**
```bash
nuget.exe push {arquivo.nupkg} {apiKey} -Source {url-do-feed}/nuget
```

Mais detalhes em [NugetServer/README.md](NugetServer/README.md).

## Convenções gerais

- A maioria dos pacotes é publicada como NuGet privado, com saída em `C:\Desenvolvimento\Nuget\Dietcode` (ver `PackageOutputPath` em cada `.csproj`) e servida localmente pelo `NugetServer`.
- Projetos "Classic"/"Net" (.NET Framework 4.8) existem para dar suporte a aplicações legadas ainda não migradas; a API é, na maioria dos casos, equivalente à versão moderna do mesmo domínio — consulte o README do par moderno para exemplos mais completos quando o legado remeter a ele.
- Licença: MIT, salvo indicação contrária no `.csproj` do projeto.
