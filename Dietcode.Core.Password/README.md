# Dietcode.Core.Password

Biblioteca para validação de senhas e verificação de senhas comprometidas,
com o provider plugável via injeção de dependência — mesmo padrão de
`Dietcode.Core.Cep`/`Dietcode.Core.Jobs`.

A senha nunca é enviada em texto puro nem como hash completo a terceiros: a
implementação V1 usa a API **range** do [Have I Been Pwned — Pwned
Passwords](https://haveibeenpwned.com/Passwords) com o modelo de
**k-anonymity** — só os 5 primeiros caracteres do SHA-1 da senha saem da
aplicação; o restante da comparação é local.

> Encontrar uma senha no HIBP significa que ela apareceu no corpus de senhas
> comprometidas. Não significa, isoladamente, que a conta específica do
> usuário foi invadida.

**SHA-1 é usado aqui somente porque faz parte do protocolo de consulta do
Pwned Passwords. Nunca deve ser usado como algoritmo de armazenamento de
senha.**

## Instalação

```bash
dotnet add package Dietcode.Core.Password --version 10.0.0
```

## Configuração

```csharp
using Dietcode.Core.Password;
using Dietcode.Core.Password.Extensions;

builder.Services.AddDietcodePassword();
```

Sem argumentos usa os padrões: HIBP como provider, `MinimumLength = 12`,
`CheckCompromisedPasswords = true`, `RejectCompromisedPasswords = true`,
`ProviderFailurePolicy = FailClosed`.

Personalizando:

```csharp
builder.Services.AddDietcodePassword(
    PasswordProviderPrimario.Hibp,
    configureValidation: options =>
    {
        options.MinimumLength = 10;
        options.ProviderFailurePolicy = ProviderFailurePolicy.FailOpen;
    },
    configureHibp: options =>
    {
        options.TimeoutSeconds = 3;
        options.CacheMinutes = 30;
    });
```

Ou lendo de `appsettings.json`:

```json
{
  "PasswordSecurity": {
    "MinimumLength": 12,
    "MaximumLength": 128,
    "CheckCompromisedPasswords": true,
    "RejectCompromisedPasswords": true,
    "ProviderFailurePolicy": "FailClosed",
    "Hibp": {
      "Enabled": true,
      "TimeoutSeconds": 5,
      "UsePadding": true,
      "CacheMinutes": 60
    }
  }
}
```

```csharp
builder.Services.AddDietcodePassword(builder.Configuration.GetSection("PasswordSecurity"));
```

## Uso

```csharp
using Dietcode.Core.Password.Abstractions;
using Dietcode.Core.Password.Models;

public sealed class ContaController
{
    private readonly IPasswordValidationService _passwordValidationService;

    public ContaController(IPasswordValidationService passwordValidationService)
    {
        _passwordValidationService = passwordValidationService;
    }

    public async Task<IActionResult> TrocarSenha(string novaSenha, string userName, string email, CancellationToken ct)
    {
        var contexto = new PasswordValidationContext { UserName = userName, Email = email };
        var resultado = await _passwordValidationService.ValidateAsync(novaSenha, contexto, ct);

        if (!resultado.IsValid)
        {
            // resultado.Issues: lista de PasswordValidationIssue (Code/Message) — tratar na UI.
            return BadRequest(resultado.Issues);
        }

        // senha aprovada: aplicar o hasher de senha da aplicação (NUNCA SHA-1) e persistir.
        return Ok();
    }
}
```

`UserName`/`Email`/`ApplicationName` do `PasswordValidationContext` servem
**somente** para regras locais (a senha não pode conter o username/e-mail) —
nunca são enviados ao provider de senha comprometida.

## Resultado

`ValidateAsync` nunca lança exceção para senha inválida, comprometida ou
indisponibilidade do provider — sempre retorna `PasswordValidationResult`:

- `IsValid`: `false` se houver qualquer `Issue` bloqueante.
- `IsCompromised` / `BreachOccurrences` / `BreachStatus`: resultado da
  verificação de comprometimento, mesmo quando não bloqueante (ex.:
  `RejectCompromisedPasswords = false`).
- `Issues`: lista de `PasswordValidationIssue` (`Code`/`Message`) — ver
  `PasswordValidationIssueCodes` para os códigos possíveis
  (`PASSWORD_REQUIRED`, `PASSWORD_TOO_SHORT`, `PASSWORD_TOO_LONG`,
  `PASSWORD_COMPROMISED`, `PASSWORD_CONTAINS_USERNAME`,
  `PASSWORD_CONTAINS_EMAIL`, `PASSWORD_PROVIDER_UNAVAILABLE`).

## Indisponibilidade não é "senha segura"

Uma falha do provider **nunca** é tratada como `Safe`. `PasswordCheckStatus`
distingue `Safe`, `Compromised`, `ProviderUnavailable`, `InvalidInput` e
`Error`. O que fazer com `ProviderUnavailable` é escolha explícita da
aplicação, via `ProviderFailurePolicy`:

- `FailClosed` (padrão): bloqueia a troca/criação da senha.
- `FailOpen`: permite continuar; `BreachStatus` continua exposto como
  `ProviderUnavailable` pra quem consome decidir o que fazer (ex.: logar,
  avisar o usuário).

## Provider HIBP

- Endpoint: `GET https://api.pwnedpasswords.com/range/{prefix}` — `prefix`
  são os 5 primeiros caracteres do SHA-1 (UTF-8, hexadecimal maiúsculo) da
  senha; o restante (`suffix`) nunca sai do processo.
- Cache por prefixo via `IMemoryCache` (`HibpOptions.CacheMinutes`, padrão
  60 min) — nunca cacheia falha, só resposta bem-sucedida.
- Um retry automático só para falha transitória (HTTP 5xx ou erro de rede);
  nunca retry para 4xx.
- `Add-Padding: true` por padrão (`HibpOptions.UsePadding`) e
  `User-Agent: Dietcode.Core.Password/1.0`.
- `HibpOptions.Enabled = false` desliga a chamada HTTP por completo
  (retorna `Safe` sem rede) — kill-switch independente de
  `PasswordValidationOptions.CheckCompromisedPasswords`.

## Preparado para novos providers (Offline, corporativo...)

Adicionar um novo provider é implementar `ICompromisedPasswordProvider` e um
novo caso em `PasswordProviderPrimario`/`AddDietcodePassword`. Diferente do
CEP, **não há contingência automática** entre providers aqui: a verificação
de senha comprometida é stateless (não tem "primário indisponível, cai pro
secundário" com o mesmo significado, já que corpora diferentes podem
discordar) — então a escolha de provider é explícita e fixa por processo. Se
no futuro dois providers precisarem coexistir com failover, o padrão de
`ContingencyCepProvider` pode ser reaproveitado.

## Scoring de força de senha

```csharp
using Dietcode.Core.Password.Scoring;

var resultado = "Senha@123".AsSpan().AnalyzePassword();

if (resultado.MeetsMinimumRules)
{
    // Senha atende as regras mínimas de composição.
}
```

A análise considera tamanho mínimo, maiúsculas, minúsculas, números,
símbolos, espaços em branco, caracteres fora de ASCII, entropia estimada e
nível de força (`PasswordStrengthLevel`: `VeryWeak` → `VeryStrong`).

Esta funcionalidade morava em `Dietcode.Core.Lib` (pasta `Passwords`) e foi
migrada para cá — é a mesma análise, sem mudança de comportamento, só de
namespace (`Dietcode.Core.Lib.Passwords` → `Dietcode.Core.Password.Scoring`).
Hoje ainda não está integrada ao fluxo de `IPasswordValidationService`; é a
base para a evolução futura de regras de criação de senha baseadas em
pontuação.

## Segurança em memória

`string` em .NET é imutável e não garante zerar a memória após o uso.
Por isso, evite cópias desnecessárias da senha, nunca a interpole em
mensagens de log/exceção, não a serialize e não a mantenha em objetos de
longa duração.

## Pacotes relacionados

- `Dietcode.Api.Core.Results`: não é usado aqui — os resultados deste
  pacote (`PasswordBreachResult`, `PasswordValidationResult`) são próprios,
  no mesmo espírito do `CepLookupResult` de `Dietcode.Core.Cep`.
- `Dietcode.Core.Security`: criptografia AES — funcionalidade distinta
  (cifra simétrica, não hashing de senha para armazenamento). Este pacote
  não substitui o hasher de senha da aplicação.

## Licença

MIT
