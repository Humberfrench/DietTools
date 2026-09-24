# SPEC --- Connection String Provider / Vault Integration

## 1. Objetivo

Evoluir o NuGet responsável pela configuração do Entity Framework da
Dietcode para permitir que as **Connection Strings sejam obtidas de
diferentes fontes**.

Atualmente, o pacote possui comportamento fixo, esperando que a
Connection String esteja disponível na configuração da aplicação através
da chave padrão:

``` text
DbContextConnString
```

O objetivo é flexibilizar esse comportamento, permitindo dois modos
iniciais:

``` text
Context
Vault
```

No modo `Context`, a Connection String continuará sendo obtida da
configuração da aplicação.

No modo `Vault`, a Connection String será obtida de um provedor externo
de Secrets.

A primeira implementação de Vault será:

``` text
Bitwarden Secrets Manager
```

A arquitetura deverá, entretanto, permitir futuramente outros providers,
como Azure Key Vault, AWS Secrets Manager, Google Secret Manager etc.

------------------------------------------------------------------------

## 2. Configuração proposta

``` json
{
  "DatabaseProvider": "Vault",
  "VaultProvider": "Bitwarden",
  "KeyContext": "DbContextConnString",
  "VaultConn": [
    {
      "Key": "Dietcode.ERP"
    },
    {
      "Key": "Dietcode.CRM"
    },
    {
      "Key": "Dietcode.Identity"
    }
  ]
}
```

> `KeyContext` e `VaultConn` pertencem a estratégias diferentes e não
> precisam ser utilizados simultaneamente.

------------------------------------------------------------------------

## 3. DatabaseProvider

A propriedade `DatabaseProvider` determina **de onde o NuGet deverá
obter a Connection String**.

Valores inicialmente suportados:

``` text
Context
Vault
```

### 3.1. DatabaseProvider = Context

Mantém o funcionamento tradicional.

A Connection String será obtida através da configuração da aplicação
(`IConfiguration`, `appsettings.json`, Environment Variables, User
Secrets etc.).

Exemplo:

``` json
{
  "DatabaseProvider": "Context",
  "KeyContext": "MinhaConnectionString"
}
```

Nesse caso, o pacote deverá procurar a Connection String utilizando
`MinhaConnectionString`.

### Comportamento padrão

`KeyContext` é opcional.

Se `KeyContext` for `null`, vazio ou branco, utilizar:

``` text
DbContextConnString
```

Portanto:

``` json
{
  "DatabaseProvider": "Context"
}
```

equivale ao comportamento:

``` json
{
  "DatabaseProvider": "Context",
  "KeyContext": "DbContextConnString"
}
```

Isso garante **retrocompatibilidade** com as aplicações existentes.

------------------------------------------------------------------------

## 4. KeyContext

`KeyContext` deverá ser utilizado **exclusivamente quando**:

``` text
DatabaseProvider = Context
```

Sua finalidade é remover o acoplamento atual do NuGet com
`DbContextConnString`.

Fluxo:

``` text
DatabaseProvider = Context
          ↓
       KeyContext
          ↓
Connection String configurada
```

Fallback:

``` text
KeyContext informado?
   ├── SIM → utilizar KeyContext
   └── NÃO → utilizar "DbContextConnString"
```

------------------------------------------------------------------------

## 5. DatabaseProvider = Vault

Quando:

``` json
"DatabaseProvider": "Vault"
```

a Connection String **não deverá ser obtida diretamente do
Context/configuração tradicional**.

O NuGet deverá utilizar um **Vault Provider**.

``` text
DatabaseProvider
      ↓
    Vault
      ↓
VaultProvider
      ↓
Bitwarden
      ↓
Secret Key
      ↓
Secret Value
      ↓
Connection String
```

Nesse modo, `KeyContext` deverá ser ignorado.

------------------------------------------------------------------------

## 6. VaultProvider

A propriedade:

``` json
"VaultProvider": "Bitwarden"
```

determina qual implementação de Vault será utilizada.

Inicialmente:

``` text
Bitwarden
```

A implementação deverá evitar acoplamento direto entre a resolução da
Connection String e o Bitwarden.

Conceitualmente:

``` text
IVaultProvider
      ├── BitwardenVaultProvider
      ├── AzureVaultProvider       [futuro]
      └── AwsVaultProvider         [futuro]
```

Assim, a lógica do EF não precisa conhecer os detalhes de cada
fornecedor.

------------------------------------------------------------------------

## 7. VaultConn

`VaultConn` representa as Connection Strings que deverão ser resolvidas
através do Vault.

``` json
"VaultConn": [
  {
    "Key": "Dietcode.ERP"
  },
  {
    "Key": "Dietcode.CRM"
  },
  {
    "Key": "Dietcode.Identity"
  }
]
```

Cada `Key` representa o **nome lógico do Secret** no Vault.

No Bitwarden Secrets Manager atualmente teremos:

``` text
Projeto DEV
├── Dietcode.ERP
├── Dietcode.CRM
└── Dietcode.Identity
```

Cada Secret possui:

``` text
key   → nome lógico
value → Connection String completa
```

O NuGet deverá trabalhar com a key lógica, por exemplo `Dietcode.ERP`, e
não com o GUID interno do Secret do Bitwarden.

------------------------------------------------------------------------

## 8. Resolução Bitwarden

``` text
VaultConn.Key
     │
     │ "Dietcode.ERP"
     ↓
BitwardenVaultProvider
     ↓
Bitwarden Secrets Manager
     ├── key   = Dietcode.ERP
     └── value = Data Source=...
                    ↓
             Connection String
```

O `value` retornado pelo Bitwarden deverá ser tratado como a
**Connection String completa**, sem necessidade de reconstrução pelo
NuGet.

------------------------------------------------------------------------

## 9. Autenticação no Bitwarden

O acesso ao Bitwarden Secrets Manager será realizado através de uma
**Machine Account**.

Para DEV foi criada:

``` text
Dietcode-DEV
```

com permissão **READ ONLY** sobre o projeto `DEV`.

``` text
Aplicação
    │
    │ Access Token
    ↓
Machine Account
Dietcode-DEV
    │
    │ READ
    ↓
Project DEV
    ├── Dietcode.ERP
    ├── Dietcode.CRM
    └── Dietcode.Identity
```

A forma definitiva de armazenamento/fornecimento do token à aplicação
será definida separadamente.

**O Access Token não deverá ser hardcoded no NuGet.**

------------------------------------------------------------------------

## 10. DEV × WEB

A separação de ambientes será feita através dos **Projects do
Bitwarden**, e não através do nome das Keys.

Estrutura planejada:

``` text
Bitwarden Secrets Manager
│
├── DEV
│   ├── Dietcode.ERP
│   ├── Dietcode.CRM
│   └── Dietcode.Identity
│
└── WEB
    ├── Dietcode.ERP
    ├── Dietcode.CRM
    └── Dietcode.Identity
```

A aplicação continuará solicitando `Dietcode.ERP` independentemente do
ambiente.

O ambiente determinará **qual Machine Account/credencial possui acesso
ao projeto correspondente**.

``` text
LOCAL / DEV
Dietcode-DEV
      ↓
Project DEV
      ↓
Dietcode.ERP
      ↓
Connection String DEV
```

Produção:

``` text
WEB
Dietcode-WEB
      ↓
Project WEB
      ↓
Dietcode.ERP
      ↓
Connection String Produção
```

Não criar chaves como:

``` text
Dietcode.ERP.DEV
Dietcode.ERP.WEB
```

A Key deverá permanecer:

``` text
Dietcode.ERP
```

------------------------------------------------------------------------

## 11. Fluxo completo --- Context

``` text
DatabaseProvider = Context
          │
          ↓
KeyContext informado?
     │            │
    SIM          NÃO
     │            │
     ↓            ↓
KeyContext   DbContextConnString
     │            │
     └──────┬─────┘
            ↓
      IConfiguration
            ↓
     Connection String
            ↓
        EF / DbContext
```

------------------------------------------------------------------------

## 12. Fluxo completo --- Vault

``` text
DatabaseProvider = Vault
          ↓
     VaultProvider
          ↓
       Bitwarden
          ↓
       VaultConn
          ↓
   Key = Dietcode.ERP
          ↓
     Secret no Vault
          ↓
         Value
          ↓
   Connection String
          ↓
      EF / DbContext
```

------------------------------------------------------------------------

## 13. Regras funcionais

1.  `DatabaseProvider` determina a estratégia de obtenção da Connection
    String.
2.  `DatabaseProvider = Context` utiliza configuração tradicional.
3.  `DatabaseProvider = Vault` utiliza um provedor externo de Secrets.
4.  `KeyContext` somente possui efeito quando
    `DatabaseProvider = Context`.
5.  Quando `KeyContext` não estiver definido, utilizar
    `DbContextConnString`.
6.  Isso deverá preservar o comportamento das aplicações existentes.
7.  `VaultProvider` somente possui efeito quando
    `DatabaseProvider = Vault`.
8.  Inicialmente o único `VaultProvider` suportado será `Bitwarden`.
9.  `VaultConn` poderá conter **uma ou várias Keys**.
10. Cada `VaultConn.Key` corresponde ao nome (`key`) de um Secret no
    Bitwarden.
11. O `value` desse Secret corresponde à Connection String completa.
12. A implementação não deverá depender do GUID interno do Secret.
13. Credenciais/tokens do Vault não poderão ser hardcoded no pacote.
14. A aplicação deverá falhar de forma explícita caso uma Key
    obrigatória não possa ser resolvida.
15. Nunca registrar em log:
    -   Connection String completa;
    -   senha do banco;
    -   Access Token do Bitwarden;
    -   `value` de Secrets.

------------------------------------------------------------------------

## 14. Retrocompatibilidade

Aplicações existentes que atualmente dependem de:

``` text
DbContextConnString
```

não deverão obrigatoriamente alterar sua configuração.

O comportamento legado deverá continuar equivalente a:

``` text
DatabaseProvider = Context
KeyContext = DbContextConnString
```

A evolução deve **flexibilizar o pacote**, não quebrar os consumidores
existentes.

------------------------------------------------------------------------

## 15. Resultado esperado

Ao final, o NuGet de EF da Dietcode deixará de possuir dependência
rígida de `DbContextConnString` e passará a possuir uma estratégia de
resolução:

``` text
                    Connection Resolver
                           │
             ┌─────────────┴─────────────┐
             │                           │
          Context                       Vault
             │                           │
        KeyContext                  VaultProvider
             │                           │
   IConfiguration                   Bitwarden
             │                           │
             │                       VaultConn
             │                           │
             └─────────────┬─────────────┘
                           ↓
                   Connection String
                           ↓
                      EF Core
```

**Primeira implementação:**

``` text
Context + Bitwarden Secrets Manager
```

**Arquitetura preparada para:**

``` text
Context
Bitwarden
Azure Key Vault
AWS Secrets Manager
outros providers
```

O resolver deverá encontrar a Connection String e, dali em diante, o
restante da aplicação e o `DbContext` deverão seguir o fluxo normal.
