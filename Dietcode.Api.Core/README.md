# Dietcode.Api.Core Basics

Biblioteca auxiliar para padronização de APIs na Dietcode, oferecendo componentes reutilizáveis para controle de resultados, tratamento de erros, ProblemDetails, extensões MVC e suporte a middlewares como logging e rate limiting.

> _“APIs consistentes produzem sistemas mais previsíveis, mais seguros e mais fáceis de manter.”_

*MElhorias do RateLimiter => por tempo que falta

---

## 🚀 Instalação

Via **.NET CLI**:

```bash
dotnet add package Dietcode.Api.Core --version 2.6.0


##USO =>
Se for usar o [AllowDebugAuthAttribute] , é preciso adicionar o código abaixo no Program.cs:
```csharp
#if DEBUG
builder.Services.AddAuthentication("DebugAuth")
    .AddScheme<AuthenticationSchemeOptions, DebugAuthenticationHandler>("DebugAuth", null);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, DebugAuthorizationPolicyProvider>();
#endif
