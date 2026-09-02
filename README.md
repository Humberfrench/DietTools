# DietTools

Monorepositório com as bibliotecas e ferramentas internas da Dietcode: um conjunto de pacotes .NET reutilizáveis entre os produtos da empresa, cobrindo API REST (ASP.NET Core), validação e regras de domínio, acesso a dados, jobs em background, e-mail, criptografia, QR Code e utilitários gerais — além das versões legadas equivalentes para projetos ainda em .NET Framework 4.8.

Cada projeto tem seu próprio README com objetivo, funcionalidades e exemplos de uso. Este documento serve como índice e mapa da arquitetura da solução.

## Como abrir

A solução principal é [`Dietcode.Api.Core.sln`](Dietcode.Api.Core.sln) — reúne a maior parte dos projetos, organizados nas pastas de solução descritas abaixo. `Dietcode.Api.RestHelper`, `Tools` e `NugetServer` têm `.sln` próprios ou não fazem parte dela ainda.

## 00 — API REST

Camada de infraestrutura HTTP para APIs ASP.NET Core: converte o resultado de aplicação em resposta HTTP.

| Projeto | Descrição |
|---|---|
| [Dietcode.Api.Core](Dietcode.Api.Core/README.md) | Controller base, conversão de `MethodResult` em `IActionResult`/`ProblemDetails`, logging estruturado de requests/responses e rate limiting por endpoint. |
| [Dietcode.Api.Core.Results](Dietcode.Api.Core.Results/README.md) | Modelo padronizado de resultado (`MethodResult`, `OkResult`, `BadRequestResult` etc.) usado pela camada de aplicação, sem depender de ASP.NET Core. |
| [Dietcode.Api.RestHelper](Dietcode.Api.RestHelper/README.md) | Helper para chamadas HTTP a APIs externas baseado em `RestSharp` (não faz parte da solução principal). |

## 01 — Bibliotecas (.NET moderno)

| Projeto | Descrição |
|---|---|
| [Dietcode.Core.Lib](Dietcode.Core.Lib/README.md) | Utilitários gerais: extensões de string/data/JSON, validação de documentos brasileiros, mascaramento de dados, paginação, localização, força de senha e helper REST. |
| [Dietcode.Core.Lib.Codes](Dietcode.Core.Lib.Codes/README.md) | Geração de QR Code (motor portado do QRCoder) com renderização própria, sem `System.Drawing`. |
| [Dietcode.Core.DomainValidator](Dietcode.Core.DomainValidator/README.md) | Resultado padronizado de validação de domínio (erros, mensagens, status HTTP). |
| [Dietcode.Core.Domain.Rules](Dietcode.Core.Domain.Rules/README.md) | Composição de regras de negócio via padrão *Specification*, com expressões lambda tipadas. |
| [Dietcode.Core.Jobs](Dietcode.Core.Jobs/README.md) | Implementação de referência para jobs assíncronos em background (serviço, handler genérico, `BackgroundService`). |
| [Dietcode.Core.Jobs.Interfaces](Dietcode.Core.Jobs.Interfaces/README.md) | Contratos e modelos usados por `Dietcode.Core.Jobs` (fila, store de estado, dispatcher). |
| [Dietcode.Core.Email](Dietcode.Core.Email/README.md) | Envio de e-mail via SMTP autenticado. |
| [Dietcode.Core.Security](Dietcode.Core.Security/README.md) | Criptografia AES: AES-GCM atual e leitura de compatibilidade com o formato ECB legado. |

## 02 — Acesso a dados (.NET moderno)

| Projeto | Descrição |
|---|---|
| [Dietcode.Database](Dietcode.Database/README.md) | Acesso a dados assíncrono com Dapper/Dapper.Contrib, múltiplos bancos (SQL Server, PostgreSQL, MySQL, Oracle). |
| [Dietcode.Database.Domain](Dietcode.Database.Domain/README.md) | Contratos (repositório, unit of work, contexto ambiente) da família baseada em EF Core. |
| [Dietcode.Database.Orm](Dietcode.Database.Orm/README.md) | Implementação em EF Core dos contratos de `Dietcode.Database.Domain`, com Dapper para SQL cru e logging Serilog. |
| [Dietcode.Database.Classic](Dietcode.Database.Classic/README.md) | Wrapper mais simples sobre EF Core, um `DbContext` por entidade, sem Unit of Work explícito — apesar do nome, não é o legado .NET Framework. |

## 12 — Acesso a dados (legado, .NET Framework 4.8)

| Projeto | Descrição |
|---|---|
| [Dietcode.Database.Net.Domain](Dietcode.Database.Net.Domain/README.md) | Contratos equivalentes a `Dietcode.Database.Domain`, para aplicações ASP.NET clássicas. |
| [Dietcode.Database.Net.Orm](Dietcode.Database.Net.Orm/README.md) | Implementação com Entity Framework 6 + Dapper dos contratos de `Dietcode.Database.Net.Domain`. |

## 11 — Bibliotecas (legado, .NET Framework 4.8)

| Projeto | Descrição |
|---|---|
| [Dietcode.Classic.Lib](Dietcode.Classic.Lib/README.md) | Versão para .NET Framework 4.8 de `Dietcode.Core.Lib`, com componentes extras para ASP.NET MVC clássico. |
| [Dietcode.Classic.Domain.Rules](Dietcode.Classic.Domain.Rules/README.md) | Versão para .NET Framework 4.8 de `Dietcode.Core.Domain.Rules`. |
| [Dietcode.Classic.DomainValidator](Dietcode.Classic.DomainValidator/README.md) | Versão para .NET Framework 4.8 de `Dietcode.Core.DomainValidator`. |

## 99 — Ferramentas e infraestrutura

Não são pacotes publicados para consumo externo; são apps internos de apoio ao desenvolvimento.

| Projeto | Descrição |
|---|---|
| [Tools](Tools/README.md) | Biblioteca placeholder (`netstandard2.0`), ainda vazia. |
| [NugetServer](NugetServer/README.md) | Servidor NuGet privado local (ASP.NET + `NuGet.Server`) para hospedar os pacotes `.nupkg` internos durante o desenvolvimento. |

## Convenções gerais

- A maioria dos pacotes é publicada como NuGet privado, com saída em `C:\Desenvolvimento\Nuget\Dietcode` (ver `PackageOutputPath` em cada `.csproj`) e servida localmente pelo `NugetServer`.
- Projetos "Classic"/"Net" (.NET Framework 4.8) existem para dar suporte a aplicações legadas ainda não migradas; a API é, na maioria dos casos, equivalente à versão moderna do mesmo domínio — consulte o README do par moderno para exemplos mais completos quando o legado remeter a ele.
- Licença: MIT, salvo indicação contrária no `.csproj` do projeto.
