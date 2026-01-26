# Dietcode Framework

Este repositório reúne **componentes de infraestrutura reutilizáveis** para APIs em ASP.NET Core, focados em **simplicidade, performance e padronização**.

O framework é dividido em dois grandes blocos:

- **Dietcode.Database** → Acesso a dados com Dapper (async-first)
- **Dietcode.Api.Core** → Middlewares utilitários (logging, rate limit, etc.)

---

## ✨ Visão Geral

### Principais objetivos

- Padronizar acesso a dados e infraestrutura HTTP
- Evitar acoplamento excessivo (DI-first)
- Facilitar observabilidade (logs estruturados)
- Ser simples de usar e simples de manter

---

# 🌐 Dietcode.Api.Core

Conjunto de **middlewares e utilitários HTTP** para APIs ASP.NET Core.

---

## 🪵 Logging de Requisições e Respostas

### 1️⃣ Logging simples (TXT)

Middleware para log completo de request/response em texto.

Captura:
- Data e hora
- Método e URL
- Body do request
- Body do response
- Status Code

Uso:

```csharp
app.UseMiddleware<RequestResponseLoggingMiddleware>();
```

---

### 2️⃣ Logging estruturado (JSON + Mask)

Middleware mais avançado, com logs em **JSON Lines (.jsonl)** e **mask automático** de dados sensíveis.

#### Configuração

`appsettings.json`

```json
{
  "ApiLogging": {
    "Directory": "logs",
    "Enabled": true
  }
}
```

Registro no DI:

```csharp
builder.Services.AddApiLogging(builder.Configuration);
```

Ativação:

```csharp
app.UseApiLogging();
```

#### Exemplo de log

```json
{
  "timestamp": "2026-01-16T14:33:21Z",
  "method": "POST",
  "url": "/api/users",
  "statusCode": 201,
  "traceId": "c3b1c2e2c6b24f0a9e8c1c1a",
  "request": { "email": "user@email.com", "password": "***" },
  "response": { "id": 10 }
}
```

Campos sensíveis são mascarados automaticamente:
- password / senha
- token / accessToken / refreshToken

---

## 🚦 Rate Limiting

Rate limit simples em memória, baseado em IP + endpoint.

### Attribute

```csharp
[RateLimit(10, 60)]
public IActionResult Get()
{
    ...
}
```

### Comportamento

- Limite por janela de tempo
- Retorno de `Retry-After`
- Payload padronizado (`RateLimitResult`)

---

## 🧠 Filosofia do Framework

- ❌ Sem enums para decidir infraestrutura
- ❌ Sem lógica escondida em repositórios
- ❌ Sem herança acidental

- ✅ DI como ponto central
- ✅ Composição > herança
- ✅ Código previsível e explícito

---

## 📄 Licença

MIT

---

## 🤝 Contribuição

Contribuições são bem-vindas.

Diretrizes:
- Respeite o padrão async-only
- Evite lógica de negócio na infraest