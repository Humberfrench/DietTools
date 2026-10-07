# Regras de versionamento — DietTools

Adaptado do modelo de `E:/Dev.Dietcode/MDs Regras/Versionamento.md` pra este
monorepo multi-pacote. A regra específica de outros produtos (ex.: Grupag
Gateway, `X` fixo por produto) não se aplica aqui — ver diferenças abaixo.

## Formato X.Y.Z

- **`X`** — major version, espelha a major version do .NET de destino dos
  projetos (`net10.0` hoje → `X = 10`). Só muda quando **todo o repositório**
  migrar de target framework (ex.: para `net11.0`). Nesse momento `X` vira
  `11` em todos os pacotes de uma vez, e cada um reinicia em `11.0.0` —
  reinício total, inclusive `Y` e `Z`.
- **`Y`** — subversão. Bump manual quando uma mudança relevante no pacote
  justifica (nova feature pública, mudança de API, etc.).
- **`Z`** — build incremental dentro da subversão atual **daquele pacote**.
  Cada entrega que altera o pacote incrementa `Z` em uma unidade. Quando `Y`
  sobe, `Z` volta a `0`.

### Diferença do modelo de origem

O modelo original é de um produto único: todos os `.csproj` avançam juntos,
em lockstep, porque são partes do mesmo produto. Aqui não — o DietTools é um
monorepo de **pacotes NuGet independentes** (`Dietcode.Core.Cep`,
`Dietcode.Core.Jobs`, `Dietcode.Core.Lib`, etc.), cada um publicado e
consumido separadamente. Por isso:

- `X` é compartilhado entre todos os pacotes (reflete o target framework
  comum a todo o repo).
- `Y.Z` avançam **de forma independente por pacote** — uma entrega que só
  toca `Dietcode.Core.Cep` incrementa a versão só dele; os outros ~37
  projetos da solução ficam como estavam.

## Projetos .NET

Manter sincronizados, em todo `.csproj` do(s) pacote(s) alterado(s):

```xml
<Version>10.Y.Z</Version>
<AssemblyVersion>10.Y.Z</AssemblyVersion>
<FileVersion>10.Y.Z.0</FileVersion>
```

O quarto número de `FileVersion` fica reservado (sempre `0`), salvo decisão
explícita em contrário. `Version`, `AssemblyVersion` e `FileVersion` nunca
destoam entre si — isso já causou bug real neste repo (`AssemblyVersion`
copiado de outro pacote por engano) e é o tipo de coisa que esta regra
existe pra evitar.

## Antes de cada publish manual

Incrementar `Z` (ou `Y`, se for o caso) do(s) pacote(s) alterado(s) antes de
gerar o pacote. `dotnet pack`/`dotnet publish` **não** incrementa a versão
automaticamente, mesmo reaproveitando artefatos de `bin`/`obj`.

## Quando migrar de target framework (`net10` → `net11`, etc.)

É o único cenário em que todos os `.csproj` da solução mudam de versão ao
mesmo tempo: `X` muda pra todos, e cada pacote reinicia `Y.Z` em `0.0`.

## Commits

Mensagens de commit continuam seguindo o padrão validado pelo Husky (ver
`README.md`, seção "Convenção de commits") — `<tipo>(escopo opcional):
mensagem`. O escopo, quando fizer sentido, pode nomear o pacote afetado
(ex.: `feat(cep): ...`, `fix(jobs): ...`).
