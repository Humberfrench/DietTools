# DietTools — resumo da documentação (.NET moderno)

Resumo consolidado de todos os `.md` dos projetos **.NET moderno** (`net10.0`) do repositório: o que cada pacote faz, como eles se relacionam, as regras do repositório e os pontos em que a documentação está desatualizada. Os detalhes e exemplos completos ficam no `README.md` de cada projeto; o índice geral está no [`README.md`](README.md) da raiz.

**Fora do escopo deste resumo (.NET Framework 4.8):** `Dietcode.Classic.Lib`, `Dietcode.Classic.Domain.Rules`, `Dietcode.Classic.DomainValidator`, `Dietcode.Database.Net.Domain`, `Dietcode.Database.Net.Orm` e `NugetServer`.

**Fontes lidas:** `README.md` e `VERSIONAMENTO.md` da raiz, `CLAUDE.md`, `specs/SPEC-ConnectionString-Vault-Bitwarden.md`, `THIRD-PARTY-NOTICES.md` do `Core.Lib.Codes` e os READMEs de 20 projetos.

---

## 1. Visão geral por camada

| Camada | Pacotes | Função |
|---|---|---|
| **API REST** | `Api.Core.Results`, `Api.Core` | Resultado padronizado da camada de aplicação e sua conversão em resposta HTTP. |
| **Utilitários** | `Core.Lib`, `Core.Lib.Rest`, `Core.Lib.Helpers`, `Core.Lib.Codes` | Extensões, validação de documentos, JSON, mascaramento, paginação, REST e QR Code. |
| **Domínio** | `Core.DomainValidator`, `Core.Domain.Rules` | Resultado de validação e regras de negócio no padrão *Specification*. |
| **Serviços plugáveis** | `Core.Cep`, `Core.Password`, `Core.Jobs` (+ `.Interfaces`), `Core.Email`, `Core.Security` | Capacidades com provider escolhido por injeção de dependência (`AddDietcode*`). |
| **Dados** | `Database`, `Database.Domain`, `Database.Orm`, `Database.Classic`, `Database.Vault` | Três famílias de acesso a dados (Dapper, EF Core com contratos, EF Core simples) e o resolvedor de connection string. |
| **Testes / ferramentas** | `UnitTests`, `Tools` | xUnit sem rede nem banco; `Tools` é um esqueleto vazio. |

## 2. Pacotes

Versões lidas dos `.csproj` (não dos READMEs — ver seção 5).

| Pacote | Versão | NuGet? | O que faz |
|---|---|---|---|
| `Api.Core.Results` | 10.12.1 | sim | `MethodResult`/`MethodResult<T>`, resultados de sucesso e erro por status HTTP, `AppServiceBase` (fábricas `Ok`, `BadRequest`, `NotFound`…), `Propagate<T>` e `ErrorBuilder`. |
| `Api.Core` | 10.10.0 | sim | `ApiControllerBase` (`Completed(result)` → `IActionResult`/`ProblemDetails`), rate limit por atributo, logging estruturado em `.jsonl` com mascaramento. |
| `Core.Lib` | 10.12.0 | sim | Extensões de string/data/JSON/enum, CPF/CNPJ, telefone/cartão/boleto, `SensitiveDataMasker`, paginação, localização. |
| `Core.Lib.Rest` | 10.1.0 | sim | `HttpService` estático (GET/POST/PUT/PATCH/DELETE), `ApiResult<T>`, headers extras. Saiu de `Core.Lib`. |
| `Core.Lib.Helpers` | — | **não** | `JsonOptionsFactory` e conversores JSON flexíveis, embutidos em `Core.Lib` e `Core.Lib.Rest`. |
| `Core.Lib.Codes` | 10.0.0 | ver seção 5 | Geração de QR Code (motor portado do QRCoder, MIT), renderização PNG própria sem `System.Drawing`, `QrEncoder`, mais de 20 payloads. Não tem Pix nem boleto. |
| `Core.DomainValidator` | 10.2.0 | sim | `ValidationResult<T>`: erros, mensagens informativas, status HTTP, `Converter<U>()`, saída em texto/HTML. |
| `Core.Domain.Rules` | 10.3.0 | sim | `ISpecification<T>`, `Rule<T>`, `Validator<T>` e especificações prontas (string preenchida, e-mail, mês/dia, número > 0…). |
| `Core.Cep` | 10.2.0 | sim | `ICepProvider` com ViaCEP e BrasilAPI, failover automático (`ContingencyCepProvider`), `CepLookupResult` que nunca lança. |
| `Core.Password` | 10.1.0 | sim | Validação de senha + verificação HIBP com k-anonymity, `ProviderFailurePolicy`, análise de força por entropia (veio de `Core.Lib`). |
| `Core.Jobs.Interfaces` | 10.1.0 | sim | Contratos: `IJob`, `IJobQueue`, `IJobHandler<T>`, `IHandlerDispatcher`, `IAsyncJobStoreGeneric`, `IJobAsyncService`. |
| `Core.Jobs` | 10.2.0 | sim | Serviço, handler e `BackgroundService` genéricos; providers em memória e Redis via `AddDietcodeJobs(primario, …)`. |
| `Core.Email` | 10.3.0 | sim | Envio SMTP com MailKit (`IEmailSender`), anexos, STARTTLS/SSL, validação sem exceção. |
| `Core.Security` | 10.0.0 | sim | AES-GCM (prefixo `v2:`, PBKDF2) com leitura de compatibilidade do AES-ECB legado; `V1.AES` marcado `[Obsolete]`. |
| `Database` | 10.3.0 | sim | Repositório Dapper/Dapper.Contrib assíncrono para SQL Server, PostgreSQL, MySQL e Oracle; `DapperUnitOfWork`; logging `.jsonl`. |
| `Database.Domain` | 10.7.0 | sim | Só contratos: `IBaseRepository<T,Tipo>`, `IMyUnitOfWork`, `IMyContextManager`, `IAmbientContextStore`, `ICompositeKey`. |
| `Database.Orm` | 10.8.0 | sim | Implementação EF Core (SQL Server) dos contratos acima, chave composta, Dapper na mesma connection string, logging Serilog. |
| `Database.Classic` | 10.3.0 | sim | EF Core simplificado, um `DbContext` por entidade, sem contratos nem Unit of Work. Apesar do nome, **não** é o legado .NET Framework. |
| `Database.Vault` | — | **não** | Resolve a connection string por `Context` (appsettings) ou `Vault` (Bitwarden Secrets Manager). Embutido em `Database.Orm`. **Sem README.** |
| `UnitTests` | — | não | xUnit sobre `Core.Lib`, `Api.Core.Results`, `Core.Email` e `Core.Cep`, com fakes escritos à mão. |
| `Tools` | — | não | `netstandard2.0`, apenas `Class1` vazia. |

### Dependências principais

```
Api.Core ──► Api.Core.Results, Core.Lib
Core.Jobs ──► Core.Jobs.Interfaces ──► Api.Core.Results
Database.Orm ──► Database.Domain ──► Core.DomainValidator
Database.Orm ──(embute)──► Database.Vault ──► Bitwarden.Secrets.Sdk
Core.Lib, Core.Lib.Rest ──(embutem)──► Core.Lib.Helpers
Database ──► Core.Lib (SensitiveDataMasker)
```

## 3. Padrões transversais

- **Resultado em vez de exceção.** Cada família tem seu tipo de retorno: `MethodResult` (aplicação/API), `ValidationResult<T>` (domínio e persistência), `CepLookupResult`, `PasswordValidationResult`, `EmailSendResult`, `ApiResult<T>` (REST). Falhas esperadas viram resultado; só cancelamento (`CancellationToken`) se propaga como exceção.
- **Provider plugável por DI.** `Core.Cep`, `Core.Password` e `Core.Jobs` seguem o mesmo molde: `AddDietcode…(primario, configure…)`. Só o CEP tem failover automático. Jobs (fila e store têm estado) e Password (corpora diferentes podem divergir) exigem escolha explícita e fixa por processo.
- **Projeto satélite não publicável.** `Core.Lib.Helpers` e `Database.Vault` têm `IsPackable=false`. Quem os referencia precisa de **três** coisas no próprio `.csproj`: `ProjectReference` com `PrivateAssets=all`, o target `CopyProjectReferencesToPackage` (embute a DLL) e, se o satélite tiver dependência NuGet real (caso do Bitwarden SDK), um `PackageReference` direto também. Sem `PrivateAssets=all` o `.nuspec` ganha uma dependência fantasma e o restore de quem instala falha com `NU1101`. Depois de empacotar, conferir o `.nuspec`.
- **Contratos separados da implementação.** `Core.Jobs.Interfaces` / `Core.Jobs` e `Database.Domain` / `Database.Orm` seguem a mesma divisão: interfaces num pacote, implementação em outro.
- **Dados sensíveis.** `SensitiveDataMasker` (de `Core.Lib`) mascara logs em `Api.Core` e `Database`. Senha nunca sai da aplicação (HIBP recebe só 5 caracteres do SHA-1). Token do Bitwarden nunca vai para o `appsettings.json` nem para log.
- **Retrocompatibilidade.** Apps que só atualizam o pacote continuam lendo `ConnectionStrings:DbContextConnString`, sem seção `DatabaseConfig`. Isso foi testado.

## 4. Regras do repositório

- **Versionamento** (`VERSIONAMENTO.md`): `X.Y.Z`, com `X` = major do target framework (hoje 10). `Y.Z` avançam **por pacote**, de forma independente. Toda entrega que altera um pacote incrementa o `Z` dele; mudança relevante sobe `Y` e zera `Z`. `Version`, `AssemblyVersion` e `FileVersion` ficam sincronizados no `.csproj`, com `FileVersion` terminando em `.0`. Migrar de framework é o único caso em que todos mudam juntos (`X` sobe, `Y.Z` reiniciam em `0.0`). `dotnet pack` não incrementa nada sozinho.
- **Commits** (Husky.Net, hook `commit-msg`): `<tipo>(escopo opcional): mensagem`, com tipos `feat|fix|chore|docs|refactor|test|style|perf|ci|build`. O escopo pode nomear o pacote (`fix(jobs): …`). O hook se instala no primeiro `dotnet restore`; desativa em CI com `HUSKY=0`.
- **Pacotes** saem em `C:\Desenvolvimento\Nuget\Dietcode` (`PackageOutputPath`). Licença MIT, salvo indicação no `.csproj`.
- **Solução principal:** `Dietcode.Api.Core.sln`.

## 5. Comportamentos e armadilhas documentados

- **`Core.Jobs` + Redis:** fila FIFO *at-most-once* (`LPUSH`/`BRPOP`), sem redelivery nem dead-letter; se o worker cair no meio de um job, ele se perde. O cancelamento só é observado entre dois `BRPOP`, então a latência máxima é `DequeuePollingSeconds`. `syncTimeout`/`asyncTimeout` da connection string precisam ficar acima desse valor. O store faz *read-modify-write* não atômico. O `IHandlerDispatcher` é sempre da aplicação.
- **`Core.Password`:** indisponibilidade do HIBP nunca vira "senha segura". O padrão é `FailClosed` (bloqueia); `FailOpen` libera e expõe `ProviderUnavailable`. `PasswordSecurityResult.IsValid` reflete só a validação; a força é informativa.
- **`Core.Cep`:** "CEP não encontrado" não aciona failover; só falha real do serviço aciona. A BrasilAPI não devolve `Complemento`, `Ddd`, `Gia` nem `Siafi`.
- **`Core.Security`:** `Decrypt` cai sozinho no AES-ECB legado quando falta o prefixo `v2:`. A chave passa por PBKDF2 com salt fixo interno.
- **`Core.Lib.Rest`:** `TResponse` precisa ser `class, new()`; primitivos não são suportados. Cada chamada cria um `HttpClient`. Falha de transporte pode subir como exceção.
- **`Database` (Dapper):** `LoggingRepositoryDecorator<T>` só instrumenta `GetByIdAsync`; o wrapping do decorator é manual.
- **`Database.Orm` / `Database.Vault`:** `ThisDatabase` é criado com `new T()`, fora do container de DI, então a resolução da connection string é estática e cacheada em processo (`Lazy`). Com `DatabaseProvider = Vault`: `VaultProvider` aceita só `Bitwarden`; `OrganizationId` (GUID) e `VaultConn.Key` são obrigatórios; o token vem de `BWS_ACCESS_TOKEN` ou da chave `BitwardenAccessToken` (env var ou user-secrets). Falha de configuração lança exceção explícita. `AddEnvironmentVariables()` passou a valer também para `ConnectionStrings`: uma env var `ConnectionStrings__DbContextConnString` sobrescreve o `appsettings.json` (decisão confirmada: nenhum app em produção usa isso hoje).
- **`Core.Jobs.Interfaces`:** `AsyncStartRequest<T>` está no namespace `Dietcode.Core.Jobs`, embora resida neste pacote (documentado no README).
- **`UnitTests`:** ficam fora banco de dados, `Api.Core`, `HttpService` (cria `HttpClient` internamente) e envio SMTP real.

## 6. Documentação desatualizada ou ausente

Achados da leitura cruzada dos READMEs com os `.csproj` e o código:

1. **Versões de instalação defasadas:**
   - `Core.Cep` README diz `--version 1.0.0`; o `.csproj` está em 10.2.0.
   - `Database.Domain` README diz 10.5.0; o `.csproj` está em 10.7.0.
   - `Core.Domain.Rules` README diz 10.2.0; o `.csproj` está em 10.3.0.
   - `Core.Lib.Rest` README não informa versão (10.1.0).
2. **`Core.Lib.Codes` README afirma que o projeto não gera pacote** (`GeneratePackageOnBuild` desabilitado), mas o `.csproj` atual tem `GeneratePackageOnBuild=true` e `Version` 10.0.0. O `README.md` da raiz já descreve isso corretamente (gera pacote localmente, não é publicado).
3. **`Database.Orm` README não menciona a seção `DatabaseConfig`** (modos `Context`/`Vault`, `OrganizationId`, `VaultConn`, token do Bitwarden). Ele ainda descreve só a chave `DbContextConnString`.
4. **`Database.Vault` não tem README.** `TesteApi` (projeto na solução) também não.
5. **README raiz sem entradas** para `Core.Lib.Rest`, `Core.Lib.Helpers`, `Database.Vault` e `TesteApi`. Além disso, a seção do `Core.Lib` na raiz ainda lista o `HttpService` e usa `Dietcode.Core.Lib.Rest` no exemplo, como se o REST fosse parte do `Core.Lib`.
6. **`Core.Lib` README:** a introdução ainda lista "criptografia" entre as funcionalidades, mas ela foi movida para `Core.Security`.
7. **`SPEC-ConnectionString-Vault-Bitwarden.md` está implementada, com desvios que a spec não registra:** `VaultConn` virou objeto único (a spec mostra lista), foi adicionado `OrganizationId`, e o nome da chave do token é `BitwardenAccessToken`. Vale marcar a spec como implementada e atualizar o JSON de exemplo.
8. **Acentuação:** `Api.Core`, `Api.Core.Results` e `Core.Lib` seguem sem acento ("Instalacao", "Licenca"), ao contrário dos READMEs mais novos.
9. **`Tools`** continua um esqueleto vazio; o README já avisa que deve ser atualizado quando houver código.
