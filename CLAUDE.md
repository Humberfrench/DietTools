# DietTools

Monorepo de pacotes .NET internos da Dietcode. Ver `README.md` (visão geral
de cada pacote) e `VERSIONAMENTO.md` antes de qualquer alteração que vá
gerar/mexer em pacote NuGet.

## Versionamento (sempre aplicar, sem precisar que o usuário peça)

Regras completas em [`VERSIONAMENTO.md`](VERSIONAMENTO.md). Resumo:

- Formato `X.Y.Z`. `X` espelha a major version do target framework (`net10.0`
  hoje → `X = 10`); só muda numa migração de target framework pra todo o
  repositório, e nesse caso reinicia `Y.Z` em `0.0` pra todos os pacotes.
- `Y.Z` avançam **por pacote**, de forma independente — nunca bump pacotes
  que a entrega não tocou.
- Toda entrega que altera um pacote incrementa `Z` dele em 1 (ou `Y`, se for
  mudança relevante o bastante — `Z` volta a `0` quando isso acontece).
- `Version`, `AssemblyVersion` e `FileVersion` sempre sincronizados no
  `.csproj` do pacote alterado (`FileVersion` com 4º número fixo em `0`).

## Commits

Mensagens seguem Conventional Commits, validado pelo hook `commit-msg` do
Husky.Net (`.husky/commit-msg`, ver `README.md`): `<tipo>(escopo opcional):
mensagem`, tipos `feat|fix|chore|docs|refactor|test|style|perf|ci|build`.
